﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Data.Mapping.Stores
{
    public partial class StoreMap : EntityTypeConfiguration<Store>
    {
        public override void Configure(EntityTypeBuilder<Store> builder)
        {
            //Import from Master table
            builder.ToTable(nameof(Store));
            builder.Ignore(x => x.Id);
            builder.HasKey(s => s.P_BranchNo);

            base.Configure(builder);
        }
    }
}
