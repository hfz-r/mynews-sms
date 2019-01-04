using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Data.Mapping.Settings
{
    public class ShelfLocationFormatMap : EntityTypeConfiguration<ShelfLocationFormat>
    {
        public override void Configure(EntityTypeBuilder<ShelfLocationFormat> builder)
        {
            builder.ToTable("ShelfLocationFormat");
            builder.HasKey(shelfLocationFormat => shelfLocationFormat.Id);

            builder.Property(shelfLocationFormat => shelfLocationFormat.Prefix1).HasMaxLength(10);

            builder.Property(shelfLocationFormat => shelfLocationFormat.Prefix2).HasMaxLength(10);

            builder.Property(shelfLocationFormat => shelfLocationFormat.Prefix3).HasMaxLength(10);

            builder.Property(shelfLocationFormat => shelfLocationFormat.Prefix4).HasMaxLength(10);

            builder.Property(shelfLocationFormat => shelfLocationFormat.Name1).HasMaxLength(100);

            builder.Property(shelfLocationFormat => shelfLocationFormat.Name2).HasMaxLength(100);

            builder.Property(shelfLocationFormat => shelfLocationFormat.Name3).HasMaxLength(100);

            builder.Property(shelfLocationFormat => shelfLocationFormat.Name4).HasMaxLength(100);

            base.Configure(builder);
        }
    }
}
