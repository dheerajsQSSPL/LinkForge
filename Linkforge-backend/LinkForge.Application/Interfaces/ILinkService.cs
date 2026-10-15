using LinkForge.Application.DTOs.Links;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkForge.Application.Interfaces
{
    public interface ILinkService
    {
        Task<CreateShortLinkResponse> CreateShortLinkAsync(CreateShortLinkRequest request);
        Task<string> GetOriginalUrlForRedirectAsync(string shortCode);
    }
}
