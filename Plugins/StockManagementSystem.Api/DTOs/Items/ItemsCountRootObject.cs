using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Items
{
    public class ItemsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}