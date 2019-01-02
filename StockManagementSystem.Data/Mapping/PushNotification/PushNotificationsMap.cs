using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.PushNotification;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Data.Mapping.PushNotification
{
    public partial class PushNotificationMap : EntityTypeConfiguration<PushNotifications>
    {
        public override void Configure(EntityTypeBuilder<PushNotifications> builder)
        {
            builder.ToTable("PushNotification");
            builder.HasKey(pushNotification => pushNotification.Id);

            builder.Property(pushNotification => pushNotification.Title)
                .HasMaxLength(256)
                .IsRequired();
            builder.Property(pushNotification => pushNotification.Desc)
                .HasMaxLength(256)
                .IsRequired();
            
            builder.HasOne(e => e.NotificationCategories)
                .WithMany(p => p.PushNotification)
                .HasForeignKey(e => e.NotificationCategoryId)
                .IsRequired();

            base.Configure(builder);
        }

    }
}
