using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class SalesMasterMap : EntityTypeConfiguration<SalesMaster>
    {
        public override void Configure(EntityTypeBuilder<SalesMaster> builder)
        {
            builder.ToTable(nameof(SalesMaster));

            base.Configure(builder);
        }
    }
}
