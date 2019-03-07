using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Settings;

namespace StockManagementSystem.Data.Mapping.Settings
{
    public partial class ReplenishmentMap : EntityTypeConfiguration<Replenishment>
    {
        public override void Configure(EntityTypeBuilder<Replenishment> builder)
        {
            builder.ToTable("Replenishment");
            builder.HasKey(replenishment => replenishment.Id);

            base.Configure(builder);
        }
    }
}
