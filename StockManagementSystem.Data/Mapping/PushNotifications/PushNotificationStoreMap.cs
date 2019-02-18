using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.PushNotifications;

namespace StockManagementSystem.Data.Mapping.PushNotifications
{
    public class PushNotificationStoreMap : EntityTypeConfiguration<PushNotificationStore>
    {
        public override void Configure(EntityTypeBuilder<PushNotificationStore> builder)
        {
            builder.ToTable("PushNotificationStore");
            builder.HasKey(pushNotificationStore => pushNotificationStore.Id);

            builder.HasOne(s => s.Store)
                .WithMany(p => p.PushNotificationStores)
                .HasForeignKey(s => s.StoreId)
                .IsRequired();

            builder.HasOne(e => e.PushNotification)
                .WithMany(p => p.PushNotificationStores)
                .HasForeignKey(e => e.PushNotificationId)
                .IsRequired();


        }
    }
}
