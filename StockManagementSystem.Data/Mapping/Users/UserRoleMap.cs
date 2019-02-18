using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Data.Mapping.Users
{
    /// <summary>
    /// Represents a user-role mapping configuration
    /// </summary>
    public class UserRoleMap : EntityTypeConfiguration<UserRole>
    {
        public override void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable(nameof(UserRole));
            builder.HasKey(mapping => new { mapping.UserId, mapping.RoleId });

            builder.Property(mapping => mapping.UserId).HasColumnName("User_Id");
            builder.Property(mapping => mapping.RoleId).HasColumnName("Role_Id");

            builder.HasOne(mapping => mapping.User)
                .WithMany(user => user.UserRoles)
                .HasForeignKey(mapping => mapping.UserId)
                .IsRequired();

            builder.HasOne(mapping => mapping.Role)
                .WithMany()
                .HasForeignKey(mapping => mapping.RoleId)
                .IsRequired();

            builder.Ignore(mapping => mapping.Id);

            base.Configure(builder);
        }
    }
}