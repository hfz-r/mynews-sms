using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Tenants;

namespace StockManagementSystem.Data.Mapping.Tenants
{
    public class TenantMappingMap : EntityTypeConfiguration<TenantMapping>
    {
        public override void Configure(EntityTypeBuilder<TenantMapping> builder)
        {
            builder.ToTable(nameof(TenantMapping));
            builder.HasKey(tenantMapping => tenantMapping.Id);

            builder.Property(tenantMapping => tenantMapping.EntityName).HasMaxLength(400).IsRequired();

            builder.HasOne(tenantMapping => tenantMapping.Tenant)
                .WithMany()
                .HasForeignKey(tenantMapping => tenantMapping.TenantId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}