using LinkForge.Application.DTOs.Links;
using LinkForge.Application.Exceptions;
using LinkForge.Application.Interfaces;
using LinkForge.Application.Services;
using LinkForge.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace LinkForge.Tests.Services
{
    public class LinkServiceTests
    {
        private readonly Mock<IShortLinkRepository> _repositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly LinkService _sut;

        public LinkServiceTests()
        {
            _repositoryMock = new Mock<IShortLinkRepository>();
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock
                .Setup(x => x["LinkForge:BaseUrl"])
                .Returns("https://localhost:7118");

            _repositoryMock
                .Setup(x => x.GetActiveByOriginalUrlAsync(
                    It.IsAny<string>()))
                .ReturnsAsync((ShortLink?)null);

            _repositoryMock
                .Setup(x => x.ExistsByShortCodeAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(false);

            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<ShortLink>()))
                .Returns(Task.CompletedTask);

            _repositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _sut = new LinkService(
                _repositoryMock.Object,
                _configurationMock.Object);
        }

        // 1. Successful short URL creation

        [Fact]
        public async Task CreateShortLinkAsync_ValidUrl_ReturnsShortUrl()
        {
            var request = new CreateShortLinkRequest
            {
                OriginalUrl = "https://example.com/products/123"
            };

            ShortLink? savedLink = null;

            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<ShortLink>()))
                .Callback<ShortLink>(link => savedLink = link)
                .Returns(Task.CompletedTask);

            var before = DateTimeOffset.UtcNow;

            var response = await _sut.CreateShortLinkAsync(request);

            var after = DateTimeOffset.UtcNow;

            Assert.Equal(
                "Short URL created successfully.",
                response.Message);

            Assert.StartsWith(
                "https://localhost:7118/",
                response.ShortUrl);

            Assert.True(response.ExpiresAt >= before.AddDays(7));
            Assert.True(response.ExpiresAt <= after.AddDays(7));

            Assert.NotNull(savedLink);
            Assert.Equal(request.OriginalUrl, savedLink.OriginalUrl);
            Assert.Equal(0, savedLink.ClickCount);
            Assert.False(savedLink.IsDeleted);

            _repositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        // 2. Invalid URL cases

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("not-a-url")]
        [InlineData("ftp://example.com/file")]
        public async Task CreateShortLinkAsync_InvalidUrl_ThrowsValidationException(
            string originalUrl)
        {
            var request = new CreateShortLinkRequest
            {
                OriginalUrl = originalUrl
            };

            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.CreateShortLinkAsync(request));

            _repositoryMock.Verify(
                x => x.AddAsync(It.IsAny<ShortLink>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateShortLinkAsync_UrlTooLong_ThrowsValidationException()
        {
            var request = new CreateShortLinkRequest
            {
                OriginalUrl = "https://example.com/" + new string('a', 2000)
            };

            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.CreateShortLinkAsync(request));
        }

        // 3. Duplicate original URL

        [Fact]
        public async Task CreateShortLinkAsync_ExistingActiveUrl_ThrowsConflictException()
        {
            var originalUrl = "https://example.com/existing";

            _repositoryMock
                .Setup(x => x.GetActiveByOriginalUrlAsync(originalUrl))
                .ReturnsAsync(MakeLink(
                    id: 1,
                    shortCode: "existing1",
                    originalUrl: originalUrl));

            var request = new CreateShortLinkRequest
            {
                OriginalUrl = originalUrl
            };

            await Assert.ThrowsAsync<ConflictException>(
                () => _sut.CreateShortLinkAsync(request));

            _repositoryMock.Verify(
                x => x.AddAsync(It.IsAny<ShortLink>()),
                Times.Never);
        }

        // 4. Valid custom alias

        [Fact]
        public async Task CreateShortLinkAsync_ValidCustomAlias_UsesAlias()
        {
            var request = new CreateShortLinkRequest
            {
                OriginalUrl = "https://example.com/product",
                CustomAlias = "my-product_2"
            };

            var response = await _sut.CreateShortLinkAsync(request);

            Assert.Equal(
                "https://localhost:7118/my-product_2",
                response.ShortUrl);
        }

        // 5. Invalid custom aliases

        [Theory]
        [InlineData("alias with spaces")]
        [InlineData("alias/with/slash")]
        [InlineData("alias?query")]
        [InlineData("alias.dot")]
        public async Task CreateShortLinkAsync_InvalidAlias_ThrowsValidationException(
            string alias)
        {
            var request = new CreateShortLinkRequest
            {
                OriginalUrl = "https://example.com/alias-test",
                CustomAlias = alias
            };

            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.CreateShortLinkAsync(request));
        }

        [Fact]
        public async Task CreateShortLinkAsync_AliasTooLong_ThrowsValidationException()
        {
            var request = new CreateShortLinkRequest
            {
                OriginalUrl = "https://example.com/alias-test",
                CustomAlias = "this-alias-is-too-long"
            };

            await Assert.ThrowsAsync<ValidationException>(
                () => _sut.CreateShortLinkAsync(request));
        }

        [Fact]
        public async Task CreateShortLinkAsync_TakenAlias_ThrowsConflictException()
        {
            _repositoryMock
                .Setup(x => x.ExistsByShortCodeAsync("taken"))
                .ReturnsAsync(true);

            var request = new CreateShortLinkRequest
            {
                OriginalUrl = "https://example.com/taken-alias",
                CustomAlias = "taken"
            };

            await Assert.ThrowsAsync<ConflictException>(
                () => _sut.CreateShortLinkAsync(request));

            _repositoryMock.Verify(
                x => x.AddAsync(It.IsAny<ShortLink>()),
                Times.Never);
        }

        // 6. Successful redirect and click-count increment

        [Fact]
        public async Task GetOriginalUrlForRedirectAsync_ActiveLink_ReturnsUrlAndIncrementsCount()
        {
            var link = MakeLink(
                id: 42,
                shortCode: "abc1234",
                originalUrl: "https://example.com/destination");

            _repositoryMock
                .Setup(x => x.GetByShortCodeAsync("abc1234"))
                .ReturnsAsync(link);

            var result =
                await _sut.GetOriginalUrlForRedirectAsync("abc1234");

            Assert.Equal(
                "https://example.com/destination",
                result);

            _repositoryMock.Verify(
                x => x.IncrementClickCountAsync(42),
                Times.Once);
        }

        // 7. Unknown short code

        [Fact]
        public async Task GetOriginalUrlForRedirectAsync_UnknownCode_ThrowsNotFoundException()
        {
            _repositoryMock
                .Setup(x => x.GetByShortCodeAsync("missing"))
                .ReturnsAsync((ShortLink?)null);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.GetOriginalUrlForRedirectAsync("missing"));

            _repositoryMock.Verify(
                x => x.IncrementClickCountAsync(It.IsAny<long>()),
                Times.Never);
        }

        // 8. Soft-deleted link

        [Fact]
        public async Task GetOriginalUrlForRedirectAsync_DeletedLink_ThrowsNotFoundException()
        {
            var link = MakeLink(5, "deleted1");
            link.IsDeleted = true;

            _repositoryMock
                .Setup(x => x.GetByShortCodeAsync("deleted1"))
                .ReturnsAsync(link);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.GetOriginalUrlForRedirectAsync("deleted1"));

            _repositoryMock.Verify(
                x => x.IncrementClickCountAsync(It.IsAny<long>()),
                Times.Never);
        }

        // 9. Expired link

        [Fact]
        public async Task GetOriginalUrlForRedirectAsync_ExpiredLink_ThrowsNotFoundException()
        {
            var link = MakeLink(6, "expired1");
            link.ExpirationDate = DateTimeOffset.UtcNow.AddMinutes(-1);

            _repositoryMock
                .Setup(x => x.GetByShortCodeAsync("expired1"))
                .ReturnsAsync(link);

            await Assert.ThrowsAsync<NotFoundException>(
                () => _sut.GetOriginalUrlForRedirectAsync("expired1"));

            _repositoryMock.Verify(
                x => x.IncrementClickCountAsync(It.IsAny<long>()),
                Times.Never);
        }

        private static ShortLink MakeLink(
            long id,
            string shortCode,
            string originalUrl = "https://example.com/target")
        {
            var now = DateTimeOffset.UtcNow;

            return new ShortLink
            {
                Id = id,
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                ExpirationDate = now.AddDays(7),
                IsDeleted = false,
                ClickCount = 0,
                CreatedDate = now,
                ModifiedDate = now
            };
        }
    }
}
