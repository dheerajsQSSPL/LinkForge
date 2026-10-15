using LinkForge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkForge.Application.Interfaces
{
    public interface IShortLinkRepository
    {
        Task<ShortLink?> GetActiveByOriginalUrlAsync(string originalUrl);

        Task<ShortLink?> GetByShortCodeAsync(string shortCode);

        Task<bool> ExistsByShortCodeAsync(string shortCode);

        Task AddAsync(ShortLink shortLink);

        Task SaveChangesAsync();

        Task IncrementClickCountAsync(long id);
    }
}
