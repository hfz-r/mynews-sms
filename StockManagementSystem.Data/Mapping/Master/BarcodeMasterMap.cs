using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class BarcodeMasterMap : EntityTypeConfiguration<BarcodeMaster>
    {
        public override void Configure(EntityTypeBuilder<BarcodeMaster> builder)
        {
            builder.ToTable(nameof(BarcodeMaster));

            base.Configure(builder);
        }
    }
}
