using System;
using System.Collections.Generic;
using System.Text;

namespace LinkForge.Application.DTOs.Links
{
    public class CreateShortLinkRequest
    {
        public string OriginalUrl { get; set; }
        public string? CustomAlias { get; set; }
    }
}
