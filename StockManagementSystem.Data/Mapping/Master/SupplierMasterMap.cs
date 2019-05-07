using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class SupplierMasterMap : EntityTypeConfiguration<SupplierMaster>
    {
        public override void Configure(EntityTypeBuilder<SupplierMaster> builder)
        {
            builder.ToTable(nameof(SupplierMaster));

            base.Configure(builder);
        }
    }
}
