using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Transactions;

namespace StockManagementSystem.Data.Mapping.Transactions
{
    /// <summary>
    /// Represent fake transaction mapping 
    /// </summary>
    public class TransactionMap : EntityTypeConfiguration<Transaction>
    {
        public override void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable(nameof(Transaction));
            builder.HasKey(trnx => trnx.Id);

            builder.Property(trnx => trnx.P_StockCode).HasMaxLength(400);

            builder.HasOne(trnx => trnx.Store)
                .WithMany()
                .HasForeignKey(trnx => trnx.P_BranchNo)
                .IsRequired();

            base.Configure(builder);
        }
    }
}