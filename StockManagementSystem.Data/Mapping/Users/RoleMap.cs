using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Data.Mapping.Users
{
    /// <summary>
    /// Represents a role mapping configuration
    /// </summary>
    public class RoleMap : EntityTypeConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(nameof(Role));
            builder.HasKey(role => role.Id);

            builder.Property(role => role.Name).HasMaxLength(255).IsRequired();
            builder.Property(role => role.SystemName).HasMaxLength(255);

            base.Configure(builder);
        }
    }
}