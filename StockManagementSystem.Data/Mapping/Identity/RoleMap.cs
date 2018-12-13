using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Data.Mapping.Identity
{
    public partial class RoleMap : EntityTypeConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(role => role.Id);

            builder.Property(role => role.ConcurrencyStamp).HasMaxLength(Int32.MaxValue);

            builder.Property(role => role.Name).HasMaxLength(256);
            builder.Property(role => role.NormalizedName).HasMaxLength(256);

            builder.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            builder.HasIndex(r => r.NormalizedName);

            base.Configure(builder);
        }
    }
}
