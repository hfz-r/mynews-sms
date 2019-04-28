﻿using System;
using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.PushNotifications
{
    public class PushNotification : BaseEntity, IAppendTimestamps
    {
        public string Title { get; set; }

        public string Desc { get; set; }

        public string StockTakeNo { get; set; }

        public int NotificationCategoryId { get; set; }

        public string JobName { get; set; }

        public string JobGroup { get; set; }

        public int? Interval { get; set; }

        public bool RemindMe { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public virtual NotificationCategory NotificationCategory { set; get; }

        public virtual ICollection<PushNotificationStore> PushNotificationStores { set; get; }

        #region IAppendTimestamps members

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}
