using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Security;

namespace StockManagementSystem.Data.Mapping.Security
{
    public partial class PermissionMap : EntityTypeConfiguration<Permission>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable(nameof(Permission));
            builder.HasKey(record => record.Id);

            builder.Property(record => record.Name).IsRequired();
            builder.Property(record => record.SystemName).HasMaxLength(255).IsRequired();
            builder.Property(record => record.Category).HasMaxLength(255).IsRequired();

            base.Configure(builder);
        }
    }
}