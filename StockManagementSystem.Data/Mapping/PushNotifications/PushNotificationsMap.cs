using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.PushNotifications;

namespace StockManagementSystem.Data.Mapping.PushNotifications
{
    public partial class PushNotificationMap : EntityTypeConfiguration<PushNotification>
    {
        public override void Configure(EntityTypeBuilder<PushNotification> builder)
        {
            builder.ToTable("PushNotification");
            builder.HasKey(pushNotification => pushNotification.Id);

            builder.Property(pushNotification => pushNotification.Title)
                .HasMaxLength(256)
                .IsRequired();
            builder.Property(pushNotification => pushNotification.Desc)
                .HasMaxLength(256)
                .IsRequired();
            builder.Property(pushNotification => pushNotification.StockTakeNo)
                .HasMaxLength(4)
                .IsRequired();
            builder.HasOne(e => e.NotificationCategory)
                .WithMany(p => p.PushNotification)
                .HasForeignKey(e => e.NotificationCategoryId)
                .IsRequired();

            base.Configure(builder);
        }

    }
}
