using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class ShelfLocationMasterMap : EntityTypeConfiguration<ShelfLocationMaster>
    {
        public override void Configure(EntityTypeBuilder<ShelfLocationMaster> builder)
        {
            builder.ToTable(nameof(ShelfLocationMaster));

            base.Configure(builder);
        }
    }
}
