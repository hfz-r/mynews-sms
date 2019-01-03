using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Data.Mapping.Settings
{
    public partial class OrderLimitStoreMap : EntityTypeConfiguration<OrderLimitStore>
    {
        public override void Configure(EntityTypeBuilder<OrderLimitStore> builder)
        {
            builder.ToTable("OrderLimitStore");
            builder.HasKey(orderLimitStore => orderLimitStore.Id);

            builder.HasOne(ur => ur.Store)
                .WithMany(u => u.OrderLimitStores)
                .HasForeignKey(ur => ur.StoreId)
                .IsRequired();

            builder.HasOne(ur => ur.OrderLimit)
                .WithMany(u => u.OrderLimitStores)
                .HasForeignKey(ur => ur.OrderLimitId) 
                .IsRequired();

            base.Configure(builder);
        }
    }
}
