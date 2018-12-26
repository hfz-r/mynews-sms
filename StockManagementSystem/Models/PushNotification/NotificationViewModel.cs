using StockManagementSystem.Core.Domain.PushNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Models.PushNotification
{
    public class NotificationViewModel
    {
        public IEnumerable<PushNotifications> PushNotificationsList { set; get; }

        public IEnumerable<NotificationCategory> NotificationCategories { set; get; }
    }
}
