using System;
using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.PushNotifications
{
    public class PushNotification : BaseEntity, IAppendTimestamps
    {
        private ICollection<PushNotificationStore> _pushNotificationStores;

        private ICollection<PushNotificationDevice> _pushNotificationDevices;

        public string Title { get; set; }

        public string Desc { get; set; }

        public int? StockTakeNo { get; set; }

        public int NotificationCategoryId { get; set; }

        public int? Interval { get; set; }

        public bool RemindMe { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public virtual NotificationCategory NotificationCategory { set; get; }

        public virtual ICollection<PushNotificationStore> PushNotificationStores
        {
            get => _pushNotificationStores ?? (_pushNotificationStores = new List<PushNotificationStore>());
            set => _pushNotificationStores = value;
        }

        public virtual ICollection<PushNotificationDevice> PushNotificationDevices
        {
            get => _pushNotificationDevices ?? (_pushNotificationDevices = new List<PushNotificationDevice>());
            set => _pushNotificationDevices = value;
        }

        #region IAppendTimestamps members

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}
