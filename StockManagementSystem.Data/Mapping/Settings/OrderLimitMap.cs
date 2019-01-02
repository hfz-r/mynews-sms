using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Data.Mapping.Settings
{
    public partial class OrderLimitMap : EntityTypeConfiguration<OrderLimit>
    {
        public override void Configure(EntityTypeBuilder<OrderLimit> builder)
        {
            builder.ToTable("OrderLimit");
            builder.HasKey(orderLimit => orderLimit.Id);

            base.Configure(builder);
        }
    }
}
