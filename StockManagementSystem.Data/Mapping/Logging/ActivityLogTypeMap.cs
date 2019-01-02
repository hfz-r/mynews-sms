using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Logging;

namespace StockManagementSystem.Data.Mapping.Logging
{
    public class ActivityLogTypeMap : EntityTypeConfiguration<ActivityLogType>
    {
        public override void Configure(EntityTypeBuilder<ActivityLogType> builder)
        {
            builder.ToTable(nameof(ActivityLogType));
            builder.HasKey(logType => logType.Id);

            builder.Property(logType => logType.SystemKeyword).HasMaxLength(100).IsRequired();
            builder.Property(logType => logType.Name).HasMaxLength(200).IsRequired();

            base.Configure(builder);
        }
    }
}