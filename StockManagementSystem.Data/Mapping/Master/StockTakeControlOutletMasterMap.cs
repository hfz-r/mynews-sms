using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class StockTakeControlOutletMasterMap : EntityTypeConfiguration<StockTakeControlOutletMaster>
    {
        public override void Configure(EntityTypeBuilder<StockTakeControlOutletMaster> builder)
        {
            builder.ToTable(nameof(StockTakeControlOutletMaster));

            base.Configure(builder);
        }
    }
}
