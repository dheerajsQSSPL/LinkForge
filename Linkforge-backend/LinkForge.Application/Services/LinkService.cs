using LinkForge.Application.DTOs.Links;
using LinkForge.Application.Exceptions;
using LinkForge.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using EntityShortLink = LinkForge.Domain.Entities.ShortLink;

namespace LinkForge.Application.Services
{

    public class LinkService : ILinkService
    {
        private readonly IShortLinkRepository _shortLinkRepository;
        private readonly IConfiguration _configuration;

        public LinkService(
            IShortLinkRepository shortLinkRepository,
            IConfiguration configuration)
        {
            _shortLinkRepository = shortLinkRepository;
            _configuration = configuration;
        }

        public async Task<CreateShortLinkResponse> CreateShortLinkAsync(
            CreateShortLinkRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.OriginalUrl))
            {
                throw new ValidationException(
                    "Original URL cannot be null or empty.");
            }

            if (request.OriginalUrl.Length > 2000)
            {
                throw new ValidationException(
                    "Original URL cannot exceed 2000 characters.");
            }

            if (!Uri.TryCreate(
                    request.OriginalUrl,
                    UriKind.Absolute,
                    out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp &&
                 uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ValidationException(
                    "Original URL must be a valid HTTP or HTTPS URL.");
            }

            var existingLink =
                await _shortLinkRepository.GetActiveByOriginalUrlAsync(
                    request.OriginalUrl);

            if (existingLink is not null)
            {
                throw new ConflictException(
                    "A short URL already exists for this URL.");
            }

            string shortCode;

            if (!string.IsNullOrWhiteSpace(request.CustomAlias))
            {
                var customAlias = request.CustomAlias.Trim();

                if (customAlias.Length > 20)
                {
                    throw new ValidationException(
                        "Custom alias cannot exceed 20 characters.");
                }

                if (!Regex.IsMatch(customAlias, @"^[a-zA-Z0-9_-]+$"))
                {
                    throw new ValidationException(
                        "Custom alias can contain only letters, numbers, hyphens, and underscores.");
                }

                var aliasExists =
                    await _shortLinkRepository.ExistsByShortCodeAsync(
                        customAlias);

                if (aliasExists)
                {
                    throw new ConflictException(
                        $"The custom alias '{customAlias}' is already taken.");
                }

                shortCode = customAlias;
            }
            else
            {
                do
                {
                    shortCode = GenerateRandomShortCode();
                }
                while (await _shortLinkRepository.ExistsByShortCodeAsync(
                    shortCode));
            }

            var now = DateTimeOffset.UtcNow;

            var shortLink = new EntityShortLink
            {
                OriginalUrl = request.OriginalUrl,
                ShortCode = shortCode,
                ExpirationDate = now.AddDays(7),
                IsDeleted = false,
                ClickCount = 0,
                CreatedDate = now,
                ModifiedDate = now
            };

            await _shortLinkRepository.AddAsync(shortLink);
            await _shortLinkRepository.SaveChangesAsync();

            var baseUrl = _configuration["LinkForge:BaseUrl"]
                ?.TrimEnd('/');

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException(
                    "LinkForge:BaseUrl is not configured.");
            }

            return new CreateShortLinkResponse
            {
                Message = "Short URL created successfully.",
                ShortUrl = $"{baseUrl}/{shortCode}",
                ExpiresAt = shortLink.ExpirationDate
            };
        }

        public async Task<string> GetOriginalUrlForRedirectAsync(
            string shortCode)
        {
            if (string.IsNullOrWhiteSpace(shortCode))
            {
                throw new NotFoundException("Short URL was not found.");
            }

            var shortLink =
                await _shortLinkRepository.GetByShortCodeAsync(shortCode);

            if (shortLink is null)
            {
                throw new NotFoundException("Short URL was not found.");
            }

            if (shortLink.IsDeleted)
            {
                throw new NotFoundException(
                    "This short URL is no longer available.");
            }

            if (shortLink.ExpirationDate <= DateTimeOffset.UtcNow)
            {
                throw new NotFoundException(
                    "This short URL has expired.");
            }

            await _shortLinkRepository.IncrementClickCountAsync(
                shortLink.Id);

            return shortLink.OriginalUrl;
        }

        private static string GenerateRandomShortCode()
        {
            const string characters =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                "abcdefghijklmnopqrstuvwxyz" +
                "0123456789";

            var random = new Random();

            return new string(
                Enumerable.Range(0, 7)
                    .Select(_ => characters[
                        random.Next(characters.Length)])
                    .ToArray());
        }
    }
}
