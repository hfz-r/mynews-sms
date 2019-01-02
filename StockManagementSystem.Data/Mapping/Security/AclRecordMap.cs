using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Security;

namespace StockManagementSystem.Data.Mapping.Security
{
    public class AclRecordMap : EntityTypeConfiguration<AclRecord>
    {
        public override void Configure(EntityTypeBuilder<AclRecord> builder)
        {
            builder.ToTable(nameof(AclRecord));
            builder.HasKey(record => record.Id);

            builder.Property(record => record.EntityName).HasMaxLength(400).IsRequired();

            builder.HasOne(record => record.Role)
                .WithMany()
                .HasForeignKey(record => record.RoleId)
                .IsRequired();

            base.Configure(builder);
        }
    }
}