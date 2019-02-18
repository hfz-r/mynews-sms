using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Services.PushNotifications
{
    public interface IPushNotificationService
    {
        void DeletePushNotification(PushNotification pushNotification);
        void DeletePushNotificationStore(int Id, Store store);
        Task<ICollection<PushNotification>> GetAllPushNotificationsAsync();
        Task<PushNotification> GetPushNotificationByIdAsync(int pushNotificationId);
        Task<IPagedList<PushNotification>> GetPushNotificationsAsync(
            int[] storeIds = null, 
            int[] pushCategoryIds = null, 
            string title = null,
            string desc = null,
            string stNo = null,
            int pageIndex = 0, 
            int pageSize = int.MaxValue, 
            bool getOnlyTotalCount = false);
        Task InsertPushNotification(PushNotification pushNotification);
        void UpdatePushNotification(PushNotification pushNotification);
        Task<IList<NotificationCategory>> GetNotificationCategoriesAsync();
    }
}