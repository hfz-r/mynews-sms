using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Data.Mapping.Identity
{
    public partial class RoleClaimMap : EntityTypeConfiguration<RoleClaim>
    {
        public override void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.ToTable("RoleClaims");
            builder.HasKey(ur => ur.Id);

            builder.HasOne(uc => uc.Role)
                .WithMany(u => u.Claims)
                .HasForeignKey(uc => uc.RoleId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}
