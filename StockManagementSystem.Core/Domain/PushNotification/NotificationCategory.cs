using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.PushNotification
{
    public class NotificationCategory : BaseEntity
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public virtual ICollection<PushNotifications> PushNotification { set; get; }
    }
}
