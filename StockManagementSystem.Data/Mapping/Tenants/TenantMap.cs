using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Tenants;

namespace StockManagementSystem.Data.Mapping.Tenants
{
    public class TenantMap : EntityTypeConfiguration<Tenant>
    {
        public override void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable(nameof(Tenant));
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name).HasMaxLength(400).IsRequired();
            builder.Property(t => t.Url).HasMaxLength(400).IsRequired();
            builder.Property(t => t.Hosts).HasMaxLength(1000);

            base.Configure(builder);
        }
    }
}