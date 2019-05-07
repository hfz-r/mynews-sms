using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class ASNDetailMasterMap : EntityTypeConfiguration<ASNDetailMaster>
    {
        public override void Configure(EntityTypeBuilder<ASNDetailMaster> builder)
        {
            builder.ToTable(nameof(ASNDetailMaster));

            base.Configure(builder);
        }
    }
}
