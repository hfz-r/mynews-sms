using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.PushNotifications
{
    public class PushNotification : Entity
    {
        public string Title { get; set; }

        public string Desc { get; set; }

        public string StockTakeNo { get; set; }

        public int NotificationCategoryId { get; set; }

        public virtual NotificationCategory NotificationCategory { set; get; }

        public virtual ICollection<PushNotificationStore> PushNotificationStores { set; get; }
    }
}
