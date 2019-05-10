using System;
using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Settings;

namespace StockManagementSystem.Api.Services
{
    public interface IOrderLimitApiService
    {
        OrderLimit GetOrderLimitById(int id);

        IList<OrderLimit> GetOrdersLimit(DateTime? createdAtMin = null, DateTime? createdAtMax = null, int limit = 50,
            int page = 1, int sinceId = 0, IList<int> storeIds = null);

        int GetOrdersLimitCount();
    }
}