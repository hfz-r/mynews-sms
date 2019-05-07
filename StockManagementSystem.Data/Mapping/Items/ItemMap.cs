using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Items;

namespace StockManagementSystem.Data.Mapping.Master
{
    public partial class ItemMap : EntityTypeConfiguration<Item>
    {
        public override void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable(nameof(Item));

            base.Configure(builder);
        }
    }
}
