using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Data.Mapping.Users
{
    /// <summary>
    /// Represents a user mapping configuration
    /// </summary>
    public class UserMap : EntityTypeConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User));
            builder.HasKey(user => user.Id);

            builder.Property(user => user.Username).HasMaxLength(1000);
            builder.Property(user => user.Email).HasMaxLength(1000);
            builder.Property(user => user.SystemName).HasMaxLength(400);

            builder.Ignore(user => user.Roles);

            base.Configure(builder);
        }
    }
}