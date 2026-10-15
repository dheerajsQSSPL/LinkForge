using LinkForge.Application.Interfaces;
using LinkForge.Domain.Entities;
using LinkForge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

using DbShortLink = LinkForge.Infrastructure.Data.ShortLink;
using EntityShortLink = LinkForge.Domain.Entities.ShortLink;

namespace LinkForge.Infrastructure.Repositories;

public class ShortLinkRepository : IShortLinkRepository
{
    private readonly LinkForgeDbContext _dbContext;

    public ShortLinkRepository(LinkForgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EntityShortLink?> GetActiveByOriginalUrlAsync(
        string originalUrl)
    {
        var entity = await _dbContext.ShortLinks
            .FirstOrDefaultAsync(x =>
                x.OriginalUrl == originalUrl &&
                !x.IsDeleted &&
                x.ExpirationDate > DateTimeOffset.UtcNow);

        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<EntityShortLink?> GetByShortCodeAsync(
        string shortCode)
    {
        var entity = await _dbContext.ShortLinks
            .FirstOrDefaultAsync(x => x.ShortCode == shortCode);

        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<bool> ExistsByShortCodeAsync(string shortCode)
    {
        return await _dbContext.ShortLinks
            .AnyAsync(x => x.ShortCode == shortCode);
    }

    public async Task AddAsync(EntityShortLink shortLink)
    {
        var entity = new DbShortLink
        {
            OriginalUrl = shortLink.OriginalUrl,
            ShortCode = shortLink.ShortCode,
            ExpirationDate = shortLink.ExpirationDate,
            IsDeleted = shortLink.IsDeleted,
            ClickCount = shortLink.ClickCount,
            CreatedDate = shortLink.CreatedDate,
            ModifiedDate = shortLink.ModifiedDate
        };

        await _dbContext.ShortLinks.AddAsync(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task IncrementClickCountAsync(long id)
    {
        await _dbContext.ShortLinks
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(
                    x => x.ClickCount,
                    x => x.ClickCount + 1));
    }

    private static EntityShortLink MapToDomain(DbShortLink entity)
    {
        return new EntityShortLink
        {
            Id = entity.Id,
            OriginalUrl = entity.OriginalUrl,
            ShortCode = entity.ShortCode,
            ExpirationDate = entity.ExpirationDate,
            IsDeleted = entity.IsDeleted,
            ClickCount = entity.ClickCount,
            CreatedDate = entity.CreatedDate,
            ModifiedDate = entity.ModifiedDate
        };
    }
}