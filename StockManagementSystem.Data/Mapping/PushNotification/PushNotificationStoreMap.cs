using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.PushNotification;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Data.Mapping.PushNotification
{
    public class PushNotificationStoreMap : EntityTypeConfiguration<PushNotificationStore>
    {
        public override void Configure(EntityTypeBuilder<PushNotificationStore> builder)
        {
            builder.ToTable("PushNotificationStores");
            builder.HasKey(pushNotificationStore => pushNotificationStore.Id);

            builder.HasOne(s => s.Store)
                .WithMany(p => p.PushNotificationStores)
                .HasForeignKey(s => s.StoreId)
                .IsRequired();

            builder.HasOne(e => e.PushNotifications)
                .WithMany(p => p.PushNotificationStores)
                .HasForeignKey(e => e.PushNotificationId)
                .IsRequired();


        }
    }
}
