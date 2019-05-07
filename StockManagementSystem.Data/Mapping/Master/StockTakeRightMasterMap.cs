using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class StockTakeRightMasterMap : EntityTypeConfiguration<StockTakeRightMaster>
    {
        public override void Configure(EntityTypeBuilder<StockTakeRightMaster> builder)
        {
            builder.ToTable(nameof(StockTakeRightMaster));

            base.Configure(builder);
        }
    }
}
