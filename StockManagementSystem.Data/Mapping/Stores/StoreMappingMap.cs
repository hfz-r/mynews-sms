using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Data.Mapping.Stores
{
    public class StoreMappingMap : EntityTypeConfiguration<StoreMapping>
    {
        public override void Configure(EntityTypeBuilder<StoreMapping> builder)
        {
            builder.ToTable(nameof(StoreMapping));
            builder.HasKey(storeMapping => storeMapping.Id);

            builder.Property(storeMapping => storeMapping.EntityName).HasMaxLength(400).IsRequired();

            builder.HasOne(storeMapping => storeMapping.Store)
                .WithMany()
                .HasForeignKey(storeMapping => storeMapping.StoreId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}