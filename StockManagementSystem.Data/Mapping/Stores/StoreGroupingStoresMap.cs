using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Data.Mapping.Stores
{
    public partial class StoreGroupingStoresMap : EntityTypeConfiguration<StoreGroupingStores>
    {
        public override void Configure(EntityTypeBuilder<StoreGroupingStores> builder)
        {
            builder.ToTable("StoreGroupingStores");
            builder.HasKey(storeGroupingStores => storeGroupingStores.Id);

            //builder.HasOne(sg => sg.StoreGroupings)
            //    .WithMany(g => g.StoreGroupingStore)
            //    .HasForeignKey(sg => sg.StoreGroupingId)
            //    .IsRequired();

            //builder.HasOne(sg => sg.Store)
            //    .WithMany(s => s.StoreGroupingStore)
            //    .HasForeignKey(sg => sg.StoreId)
            //    .IsRequired();

            base.Configure(builder);
        }
    }
}
