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

            builder.Property(shelfLocationFormat => shelfLocationFormat.Prefix).HasMaxLength(1);

            builder.Property(shelfLocationFormat => shelfLocationFormat.Name).HasMaxLength(50);

            base.Configure(builder);
        }
    }
}
