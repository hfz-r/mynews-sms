using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.OrderLimit
{
    public class OrderLimitCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}