using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.PushNotification
{
    public class PushNotificationStore : BaseEntity
    {
        public int PushNotificationId { get; set; }

        public int StoreId { get; set; }

        public bool? IsHHTDownloaded { get; set; }

        public virtual PushNotifications PushNotifications { get; set; }

        public virtual Store Store { get; set; }
    }
}
