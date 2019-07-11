using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.PushNotifications;

namespace StockManagementSystem.Data.Mapping.PushNotifications
{
    public class PushNotificationDeviceMap : EntityTypeConfiguration<PushNotificationDevice>
    {
        public override void Configure(EntityTypeBuilder<PushNotificationDevice> builder)
        {
            builder.ToTable("PushNotificationDevice");
            builder.HasKey(pushNotificationDevice => pushNotificationDevice.Id);

            builder.HasOne(s => s.Device)
                .WithMany(p => p.PushNotificationDevices)
                .HasForeignKey(s => s.DeviceId)
                .IsRequired();

            builder.HasOne(e => e.PushNotification)
                .WithMany(p => p.PushNotificationDevices)
                .HasForeignKey(e => e.PushNotificationId)
                .IsRequired();


        }
    }
}
