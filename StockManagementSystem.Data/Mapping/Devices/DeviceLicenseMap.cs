using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Devices;

namespace StockManagementSystem.Data.Mapping.Devices
{
    public class DeviceLicenseMap : EntityTypeConfiguration<DeviceLicense>
    {
        public override void Configure(EntityTypeBuilder<DeviceLicense> builder)
        {
            builder.ToTable(nameof(DeviceLicense));
            builder.HasKey(mapping => new { mapping.LicenseId, mapping.SerialNo });

            builder.Property(dl => dl.LicenseId).HasColumnName("License_Id");
            builder.Property(dl => dl.SerialNo).HasColumnName("Serial_No");

            builder.HasOne(dl => dl.Device)
                .WithMany(dev => dev.DeviceLicenses)
                .HasForeignKey(dl => dl.SerialNo)
                .HasPrincipalKey(dev => dev.SerialNo);

            builder.Ignore(dl => dl.Id);

            base.Configure(builder);
        }
    }
}