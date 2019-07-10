using System;
using System.Collections.Generic;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DTOs.PushNotifications;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Core.Domain.PushNotifications;

namespace StockManagementSystem.Api.Services
{
    public interface IPushNotificationApiService
    {
        PushNotification GetPushNotificationById(int id);

        IList<PushNotification> GetPushNotifications(DateTime? createdAtMin = null, DateTime? createdAtMax = null,
            int limit = 50, int page = 1, int sinceId = 0, IList<int> storeIds = null, DateTime? startTime = null,
            DateTime? endTime = null);

        int GetPushNotificationsCount();

        Search<PushNotificationDto> Search(
            int storeId = 0,
            string queryParams = "",
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false,
            bool count = false);
    }
}