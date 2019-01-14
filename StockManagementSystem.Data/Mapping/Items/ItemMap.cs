using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Data.Mapping.Items
{
    public class ItemMap : EntityTypeConfiguration<Item>
    {
        public override void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Item");
            builder.HasKey(item => item.Id);

            base.Configure(builder);
        }
    }
}
