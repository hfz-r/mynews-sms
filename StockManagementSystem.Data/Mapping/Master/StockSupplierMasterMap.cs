using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class StockSupplierMasterMap : EntityTypeConfiguration<StockSupplierMaster>
    {
        public override void Configure(EntityTypeBuilder<StockSupplierMaster> builder)
        {
            builder.ToTable(nameof(StockSupplierMaster));

            base.Configure(builder);
        }
    }
}
