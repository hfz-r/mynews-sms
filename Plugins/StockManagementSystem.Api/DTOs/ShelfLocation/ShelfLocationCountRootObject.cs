using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.ShelfLocation
{
    public class ShelfLocationCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}