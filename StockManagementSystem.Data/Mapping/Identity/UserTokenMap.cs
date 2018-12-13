using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Data.Mapping.Identity
{
    public partial class UserTokenMap : EntityTypeConfiguration<UserToken>
    {
        public override void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.ToTable("UserTokens");
            builder.HasKey(ut => ut.Id);

            builder.Property(ut => ut.LoginProvider).IsRequired();
            builder.Property(ut => ut.Name).IsRequired();
            builder.Property(ut => ut.Value).HasMaxLength(Int32.MaxValue);

            builder.HasOne(ut => ut.User)
                .WithMany(u => u.Tokens)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}
