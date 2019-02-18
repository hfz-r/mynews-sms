using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Transactions;

namespace StockManagementSystem.Data.Mapping.Transactions
{
    /// <summary>
    /// Represent fake branch mapping 
    /// </summary>
    public class BranchMap : EntityTypeConfiguration<Branch>
    {
        public override void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable(nameof(Branch));
            builder.HasKey(map => map.Id);

            builder.Property(map => map.Name).HasMaxLength(200);
            builder.Property(map => map.Location).HasMaxLength(400);

            base.Configure(builder);
        }
    }
}