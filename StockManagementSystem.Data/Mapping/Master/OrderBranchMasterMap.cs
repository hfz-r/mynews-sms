using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class OrderBranchMasterMap : EntityTypeConfiguration<OrderBranchMaster>
    {
        public override void Configure(EntityTypeBuilder<OrderBranchMaster> builder)
        {
            builder.ToTable(nameof(OrderBranchMaster));

            base.Configure(builder);
        }
    }
}
