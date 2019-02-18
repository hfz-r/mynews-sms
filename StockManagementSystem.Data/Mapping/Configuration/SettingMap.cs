using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Configuration;

namespace StockManagementSystem.Data.Mapping.Configuration
{
    public class SettingMap : EntityTypeConfiguration<Setting>
    {
        public override void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.ToTable(nameof(Setting));
            builder.HasKey(setting => setting.Id);

            builder.Property(setting => setting.Name).HasMaxLength(200).IsRequired();
            builder.Property(setting => setting.Value).HasMaxLength(2000).IsRequired();

            base.Configure(builder);
        }
    }
}