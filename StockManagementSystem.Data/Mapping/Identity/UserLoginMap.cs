using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Data.Mapping.Identity
{
    public partial class UserLoginMap : EntityTypeConfiguration<UserLogin>
    {
        public override void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTable("UserLogins");
            builder.HasKey(ur => ur.Id);

            builder.Property(ul => ul.LoginProvider).IsRequired();
            builder.Property(ul => ul.ProviderKey).IsRequired();
            builder.Property(ul => ul.ProviderDisplayName).HasMaxLength(Int32.MaxValue);

            builder.HasOne(ul => ul.User)
                .WithMany(u => u.Logins)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}
