using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.PushNotification;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Data.Mapping.PushNotification
{
    public partial class NotificationCategoryMap : EntityTypeConfiguration<NotificationCategory>
    {
        public override void Configure(EntityTypeBuilder<NotificationCategory> builder)
        {
            builder.ToTable("NotificationCategories");
            builder.HasKey(notificationCategory => notificationCategory.Id);

            base.Configure(builder);
        }
    }
}
