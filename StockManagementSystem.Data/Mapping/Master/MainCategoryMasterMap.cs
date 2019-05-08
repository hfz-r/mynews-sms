using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class MainCategoryMasterMap : EntityTypeConfiguration<MainCategoryMaster>
    {
        public override void Configure(EntityTypeBuilder<MainCategoryMaster> builder)
        {
            builder.ToTable(nameof(MainCategoryMaster));

            base.Configure(builder);
        }
    }
}
