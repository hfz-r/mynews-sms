using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class BranchMasterMap : EntityTypeConfiguration<BranchMaster>
    {
        public override void Configure(EntityTypeBuilder<BranchMaster> builder)
        {
            builder.ToTable(nameof(BranchMaster));

            base.Configure(builder);
        }
    }
}
