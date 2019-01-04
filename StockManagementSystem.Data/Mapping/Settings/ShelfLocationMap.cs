using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Data.Mapping.Settings
{
    public class ShelfLocationMap : EntityTypeConfiguration<ShelfLocation>
    {
        public override void Configure(EntityTypeBuilder<ShelfLocation> builder)
        {
            builder.ToTable("ShelfLocation");
            builder.HasKey(shelfLocation => shelfLocation.Id);

            builder.HasOne(e => e.ShelfLocationFormats)
                .WithMany(s => s.ShelfLocations)
                .HasForeignKey(e => e.ShelfLocationFormatId)
                .IsRequired();

            builder.HasOne(e => e.Stores)
                .WithMany(s => s.ShelfLocations)
                .HasForeignKey(e => e.StoreId)
                .IsRequired();

            //builder.HasOne(e => e.Items)
            //    .WithMany(s => s.ShelfLocations)
            //    .HasForeignKey(e => e.ItemId)
            //    .IsRequired();

            base.Configure(builder);
        }
    }
}
