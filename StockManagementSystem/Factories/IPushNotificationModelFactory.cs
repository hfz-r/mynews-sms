using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Models.PushNotifications;

namespace StockManagementSystem.Factories
{
    public interface IPushNotificationModelFactory
    {
        Task<PushNotificationModel> PreparePushNotificationListModel();
        Task<PushNotificationListModel> PreparePushNotificationListModel(PushNotificationSearchModel searchModel);
        Task<PushNotificationModel> PreparePushNotificationModel(PushNotificationModel model, PushNotification pushNotification);
        Task<PushNotificationSearchModel> PreparePushNotificationSearchModel(PushNotificationSearchModel searchModel);
    }
}