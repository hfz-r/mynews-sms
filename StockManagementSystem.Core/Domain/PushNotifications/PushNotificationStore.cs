﻿using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Core.Domain.PushNotifications
{
    public class PushNotificationStore : BaseEntity
    {
        public int PushNotificationId { get; set; }

        public int StoreId { get; set; }

        public virtual PushNotification PushNotification { get; set; }

        public virtual Store Store { get; set; }
    }
}
