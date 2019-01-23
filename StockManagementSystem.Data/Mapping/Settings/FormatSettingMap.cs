using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Data.Mapping.Settings
{
    public class FormatSettingMap : EntityTypeConfiguration<FormatSetting>
    {
        public override void Configure(EntityTypeBuilder<FormatSetting> builder)
        {
            builder.ToTable("FormatSetting");
            builder.HasKey(formatSetting => formatSetting.Id);

            builder.Property(formatSetting => formatSetting.Format).HasMaxLength(10);

            builder.Property(formatSetting => formatSetting.Prefix).HasMaxLength(1);

            builder.Property(formatSetting => formatSetting.Name).HasMaxLength(150);

            base.Configure(builder);
        }
    }
}
