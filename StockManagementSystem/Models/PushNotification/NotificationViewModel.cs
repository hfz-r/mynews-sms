using StockManagementSystem.Core.Domain.PushNotification;
using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Models.PushNotification
{
    public class NotificationViewModel
    {
        public IEnumerable<PushNotificationStore> PushNotificationStore { set; get; }
    }
}
