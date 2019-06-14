using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Directory;

namespace StockManagementSystem.Data.Mapping.Directory
{
    public partial class HolidayMap : EntityTypeConfiguration<Holiday>
    {
        public override void Configure(EntityTypeBuilder<Holiday> builder)
        {
            builder.ToTable(nameof(Holiday));
            builder.HasKey(holiday => holiday.Id);

            builder.Ignore(holiday => holiday.FullDateTime);

            builder.Property(holiday => holiday.Date).HasMaxLength(10);
            builder.Property(holiday => holiday.Day).HasMaxLength(5);
            builder.Property(holiday => holiday.Description).HasMaxLength(1000);
            builder.Property(holiday => holiday.Type).IsRequired();

            base.Configure(builder);
        }
    }
}