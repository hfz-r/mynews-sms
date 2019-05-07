using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class StockTakeControlMasterMap : EntityTypeConfiguration<StockTakeControlMaster>
    {
        public override void Configure(EntityTypeBuilder<StockTakeControlMaster> builder)
        {
            builder.ToTable(nameof(StockTakeControlMaster));

            base.Configure(builder);
        }
    }
}
