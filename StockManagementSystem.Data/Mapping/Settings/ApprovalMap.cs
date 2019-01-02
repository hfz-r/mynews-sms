using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Settings;

namespace StockManagementSystem.Data.Mapping.Approvals
{
    public partial class ApprovalMap : EntityTypeConfiguration<Approval>
    {
        public override void Configure(EntityTypeBuilder<Approval> builder)
        {
            builder.ToTable("Approval");
            builder.HasKey(approval => approval.Id);

            base.Configure(builder);
        }
    }
}
