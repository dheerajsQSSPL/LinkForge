using System;
using System.Collections.Generic;

namespace LinkForge.Infrastructure.Data;

public partial class ShortLink
{
    public long Id { get; set; }

    public string OriginalUrl { get; set; } = null!;

    public string ShortCode { get; set; } = null!;

    public DateTimeOffset ExpirationDate { get; set; }

    public bool IsDeleted { get; set; }

    public long ClickCount { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset ModifiedDate { get; set; }
}
