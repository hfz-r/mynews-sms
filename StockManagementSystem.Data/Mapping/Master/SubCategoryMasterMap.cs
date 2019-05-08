using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class SubCategoryMasterMap : EntityTypeConfiguration<SubCategoryMaster>
    {
        public override void Configure(EntityTypeBuilder<SubCategoryMaster> builder)
        {
            builder.ToTable(nameof(SubCategoryMaster));

            base.Configure(builder);
        }
    }
}
