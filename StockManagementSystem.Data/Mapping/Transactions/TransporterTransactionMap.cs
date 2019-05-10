using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Transactions;

namespace StockManagementSystem.Data.Mapping.Transactions
{
    public class TransporterTransactionMap : EntityTypeConfiguration<TransporterTransaction>
    {
        public override void Configure(EntityTypeBuilder<TransporterTransaction> builder)
        {
            builder.ToTable(nameof(TransporterTransaction));
            builder.HasKey(t => t.Id);

            builder.Property(t => t.DriverName).IsRequired();

            base.Configure(builder);
        }
    }
}