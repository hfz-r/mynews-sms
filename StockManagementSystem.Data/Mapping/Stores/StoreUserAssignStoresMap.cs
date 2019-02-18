using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Data.Mapping.Stores
{
    public partial class StoreUserAssignStoresMap : EntityTypeConfiguration<StoreUserAssignStores>
    {
        public override void Configure(EntityTypeBuilder<StoreUserAssignStores> builder)
        {
            builder.ToTable("StoreUserAssignStores");
            builder.HasKey(storeUserAssignStores => storeUserAssignStores.Id);

            builder.HasOne(us => us.StoreUserAssigns)
                .WithMany(s => s.StoreUserAssignStore)
                .HasForeignKey(us => us.StoreUserAssignId)
                .IsRequired();

            builder.HasOne(u => u.User)
                .WithMany(s => s.StoreUserAssignStore)
                .HasForeignKey(u => u.UserId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}
