using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.OrderLimit
{
    public class OrderLimitRootObject : ISerializableObject
    {
        public OrderLimitRootObject()
        {
            OrdersLimit = new List<OrderLimitDto>();
        }

        [JsonProperty("orders_limit")]
        public IList<OrderLimitDto> OrdersLimit { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "orders_limit";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(OrderLimitDto);
        }
    }
}