using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Data.Mapping.Identity
{
    public partial class UserMap : EntityTypeConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(user => user.Id);

            builder.Property(user => user.ConcurrencyStamp).HasMaxLength(Int32.MaxValue);
            builder.Property(user => user.Name).HasMaxLength(Int32.MaxValue);
            builder.Property(user => user.PasswordHash).HasMaxLength(Int32.MaxValue);
            builder.Property(user => user.SecurityStamp).HasMaxLength(Int32.MaxValue);

            builder.Property(user => user.LockoutEnd).HasMaxLength(7);
            builder.Property(user => user.Email).HasMaxLength(256);
            builder.Property(user => user.NormalizedEmail).HasMaxLength(256);
            builder.Property(user => user.NormalizedUserName).HasMaxLength(256);
            builder.Property(user => user.UserName).HasMaxLength(256);

            builder.HasMany(e => e.Logins)
                .WithOne(e => e.User)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            builder.HasMany(e => e.Tokens)
                .WithOne(e => e.User)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            builder.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.HasIndex(u => u.NormalizedUserName);
            builder.HasIndex(u => u.NormalizedEmail);

            base.Configure(builder);
        }
    }
}
