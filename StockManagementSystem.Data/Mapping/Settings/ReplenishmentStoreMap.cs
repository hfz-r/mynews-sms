using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Settings;

namespace StockManagementSystem.Data.Mapping.Settings
{
    public partial class ReplenishmentStoreMap : EntityTypeConfiguration<ReplenishmentStore>
    {
        public override void Configure(EntityTypeBuilder<ReplenishmentStore> builder)
        {
            builder.ToTable("ReplenishmentStore");
            builder.HasKey(replenishmentStore => replenishmentStore.Id);

            builder.HasOne(ur => ur.Store)
                .WithMany(u => u.ReplenishmentStores)
                .HasForeignKey(ur => ur.StoreId)
                .IsRequired();

            builder.HasOne(ur => ur.Replenishment)
                .WithMany(u => u.ReplenishmentStores)
                .HasForeignKey(ur => ur.ReplenishmentId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}
