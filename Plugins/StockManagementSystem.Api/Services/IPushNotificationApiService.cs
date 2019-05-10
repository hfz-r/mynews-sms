using System;
using System.Collections.Generic;
using StockManagementSystem.Core.Domain.PushNotifications;

namespace StockManagementSystem.Api.Services
{
    public interface IPushNotificationApiService
    {
        PushNotification GetPushNotificationById(int id);

        IList<PushNotification> GetPushNotifications(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = 50, int page = 1, int sinceId = 0, IList<int> storeIds = null);

        int GetPushNotificationsCount();
    }
}