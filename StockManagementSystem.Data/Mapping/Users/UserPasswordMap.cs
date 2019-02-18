using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Data.Mapping.Users
{
    /// <summary>
    /// Represents a user password mapping configuration
    /// </summary>
    public class UserPasswordMap : EntityTypeConfiguration<UserPassword>
    {
        public override void Configure(EntityTypeBuilder<UserPassword> builder)
        {
            builder.ToTable(nameof(UserPassword));
            builder.HasKey(password => password.Id);

            builder.HasOne(password => password.User)
                .WithMany()
                .HasForeignKey(password => password.UserId)
                .IsRequired();

            builder.Ignore(password => password.PasswordFormat);

            base.Configure(builder);
        }
    }
}