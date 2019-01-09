﻿using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockManagementSystem.Services.OrderLimits
{
    public interface IOrderLimitService
    {
        void DeleteOrderLimit(OrderLimit orderLimit);
        Task<ICollection<OrderLimit>> GetAllOrderLimitsAsync();
        Task<OrderLimit> GetOrderLimitByIdAsync(int orderLimitId);
        Task<IPagedList<OrderLimit>> GetOrderLimitsAsync(int[] storeIds = null, float percentage = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);
        Task InsertOrderLimit(OrderLimit orderLimit);
        void UpdateOrderLimit(OrderLimit orderLimit);
        void DeleteOrderLimitStore(int Id, Store store);
    }
}