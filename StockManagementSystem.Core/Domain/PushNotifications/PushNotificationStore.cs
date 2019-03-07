﻿using System;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Core.Domain.PushNotifications
{
    public class PushNotificationStore : BaseEntity, IAppendTimestamps
    {
        public int PushNotificationId { get; set; }

        public int StoreId { get; set; }

        public bool? IsHHTDownloaded { get; set; }

        public virtual PushNotification PushNotification { get; set; }

        public virtual Store Store { get; set; }

        #region IAppendTimestamps members

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}
