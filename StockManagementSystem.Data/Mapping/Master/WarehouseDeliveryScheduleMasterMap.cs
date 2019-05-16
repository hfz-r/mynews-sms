using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class WarehouseDeliveryScheduleMasterMap : EntityTypeConfiguration<WarehouseDeliveryScheduleMaster>
    {
        public override void Configure(EntityTypeBuilder<WarehouseDeliveryScheduleMaster> builder)
        {
            builder.ToTable(nameof(WarehouseDeliveryScheduleMaster));

            base.Configure(builder);
        }
    }
}
