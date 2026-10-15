using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LinkForge.Infrastructure.Data;

public partial class LinkForgeDbContext : DbContext
{
    public LinkForgeDbContext(DbContextOptions<LinkForgeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ShortLink> ShortLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortLink>(entity =>
        {
            entity.HasIndex(e => e.ShortCode, "UX_ShortLinks_ShortCode").IsUnique();

            entity.Property(e => e.OriginalUrl).HasMaxLength(2000);
            entity.Property(e => e.ShortCode)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
