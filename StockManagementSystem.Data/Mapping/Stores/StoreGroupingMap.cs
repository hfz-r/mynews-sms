using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Data.Mapping.Stores
{
    public partial class StoreGroupingMap : EntityTypeConfiguration<StoreGrouping>
    {
        public override void Configure(EntityTypeBuilder<StoreGrouping> builder)
        {
            builder.ToTable("StoreGrouping");
            builder.HasKey(storeGrouping => storeGrouping.Id);

            builder.Property(storeGrouping => storeGrouping.GroupName).HasMaxLength(256);

            base.Configure(builder);
        }
    }
}
