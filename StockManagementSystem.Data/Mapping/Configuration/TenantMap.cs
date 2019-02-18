using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Configuration;

namespace StockManagementSystem.Data.Mapping.Configuration
{
    public class TenantMap : EntityTypeConfiguration<Tenant>
    {
        public override void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable(nameof(Tenant));
            builder.HasKey(tenant => tenant.Id);

            builder.Property(tenant => tenant.Name).HasMaxLength(400).IsRequired();
            builder.Property(tenant => tenant.Url).HasMaxLength(400).IsRequired();
            builder.Property(tenant => tenant.Hosts).HasMaxLength(1000);

            base.Configure(builder);
        }
    }
}