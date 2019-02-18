using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Data.Mapping.Stores
{
    public partial class StoreUserAssignMap : EntityTypeConfiguration<StoreUserAssign>
    {
        public override void Configure(EntityTypeBuilder<StoreUserAssign> builder)
        {
            builder.ToTable("StoreUserAssign");
            builder.HasKey(storeUserAssign => storeUserAssign.Id);

            builder.HasOne(su => su.Store)
                .WithMany(s => s.StoreUserAssigns)
                .HasForeignKey(su => su.StoreId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}
