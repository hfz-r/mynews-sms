using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Items;

namespace StockManagementSystem.Data.Mapping.Items
{
    public partial class ItemMap : EntityTypeConfiguration<Item>
    {
        public override void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable(nameof(Item));
            builder.HasKey(item => item.Id);

            builder.Property(item => item.P_StockCode).HasMaxLength(50);
            builder.Property(item => item.P_Desc).HasMaxLength(100);

            base.Configure(builder);
        }
    }
}
