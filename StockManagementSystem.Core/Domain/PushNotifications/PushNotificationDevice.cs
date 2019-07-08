using StockManagementSystem.Core.Domain.Devices;

namespace StockManagementSystem.Core.Domain.PushNotifications
{
    public class PushNotificationDevice : BaseEntity
    {
        public int PushNotificationId { get; set; }

        public int DeviceId { get; set; }

        public virtual PushNotification PushNotification { get; set; }

        public virtual Device Device { get; set; }

        public string JobName { get; set; }

        public string JobGroup { get; set; }
    }
}
