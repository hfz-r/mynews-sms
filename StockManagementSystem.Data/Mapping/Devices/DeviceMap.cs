using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.Devices;

namespace StockManagementSystem.Data.Mapping.Devices
{
    public partial class DeviceMap : EntityTypeConfiguration<Device>
    {
        public override void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.ToTable("Devices");
            builder.HasKey(device => device.Id);

            builder.Property(device => device.SerialNo)
                .HasMaxLength(256)
                .IsRequired();
            builder.Property(device => device.ModelNo)
                .HasMaxLength(256)
                .IsRequired();

            base.Configure(builder);
        }
    }
}
