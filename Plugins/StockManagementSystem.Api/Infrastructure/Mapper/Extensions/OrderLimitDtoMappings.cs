using StockManagementSystem.Api.DTOs.OrderLimit;
using StockManagementSystem.Core.Domain.Settings;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class OrderLimitDtoMappings
    {
        public static OrderLimitDto ToDto(this OrderLimit orderLimit)
        {
            return orderLimit.MapTo<OrderLimit, OrderLimitDto>();
        }
    }
}