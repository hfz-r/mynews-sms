using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Security;

namespace StockManagementSystem.Data.Mapping.Security
{
    public partial class PermissionRolesMap : EntityTypeConfiguration<PermissionRoles>
    {
        public override void Configure(EntityTypeBuilder<PermissionRoles> builder)
        {
            builder.ToTable(nameof(PermissionRoles));
            builder.HasKey(mapping => new {mapping.PermissionId, mapping.RoleId});

            builder.HasOne(mapping => mapping.Role)
                .WithMany(role => role.PermissionRoles)
                .HasForeignKey(mapping => mapping.RoleId)
                .IsRequired();

            builder.HasOne(mapping => mapping.Permission)
                .WithMany(permission => permission.PermissionRoles)
                .HasForeignKey(mapping => mapping.PermissionId)
                .IsRequired();

            builder.Ignore(mapping => mapping.Id);

            base.Configure(builder);
        }
    }
}