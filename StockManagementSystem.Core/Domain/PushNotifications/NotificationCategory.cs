using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.PushNotifications
{
    public class NotificationCategory : BaseEntity
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public virtual ICollection<PushNotification> PushNotification { set; get; }
    }
}
