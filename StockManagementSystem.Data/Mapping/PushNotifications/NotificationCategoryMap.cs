using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockManagementSystem.Core.Domain.PushNotifications;

namespace StockManagementSystem.Data.Mapping.PushNotifications
{
    public partial class NotificationCategoryMap : EntityTypeConfiguration<NotificationCategory>
    {
        public override void Configure(EntityTypeBuilder<NotificationCategory> builder)
        {
            builder.ToTable("NotificationCategory");
            builder.HasKey(notificationCategory => notificationCategory.Id);

            base.Configure(builder);
        }
    }
}
