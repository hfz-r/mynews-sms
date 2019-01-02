using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.PushNotification
{
    public partial class PushNotifications : BaseEntity
    {
        public string Title { get; set; }

        public string Desc { get; set; }

        public string IsShift { get; set; }

        public int NotificationCategoryId { get; set; }

        public virtual NotificationCategory NotificationCategories { set; get; }

        public virtual ICollection<PushNotificationStore> PushNotificationStores { set; get; }
    }
}
