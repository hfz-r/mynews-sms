using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class ShiftControlMasterMap : EntityTypeConfiguration<ShiftControlMaster>
    {
        public override void Configure(EntityTypeBuilder<ShiftControlMaster> builder)
        {
            builder.ToTable(nameof(ShiftControlMaster));

            base.Configure(builder);
        }
    }
}
