using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class ASNHeaderMasterMap : EntityTypeConfiguration<ASNHeaderMaster>
    {
        public override void Configure(EntityTypeBuilder<ASNHeaderMaster> builder)
        {
            builder.ToTable(nameof(ASNHeaderMaster));

            base.Configure(builder);
        }
    }
}
