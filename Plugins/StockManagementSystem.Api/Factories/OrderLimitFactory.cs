using System;
using StockManagementSystem.Core.Domain.Settings;

namespace StockManagementSystem.Api.Factories
{
    public class OrderLimitFactory : IFactory<OrderLimit>
    {
        public OrderLimit Initialize()
        {
            var defaultOrderLimit = new OrderLimit()
            {
                DeliveryPerWeek = 0,
                InventoryCycle = 0,
                OrderRatio = 0,
                Safety = 0,
                CreatedOnUtc = DateTime.UtcNow,
            };

            return defaultOrderLimit;
        }
    }
}