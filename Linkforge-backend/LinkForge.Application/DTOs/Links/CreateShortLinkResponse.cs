using System;
using System.Collections.Generic;
using System.Text;

namespace LinkForge.Application.DTOs.Links
{
    public class CreateShortLinkResponse
    {
        public string Message { get; set; } = null!;
        public string ShortUrl { get; set; } = null!;
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
