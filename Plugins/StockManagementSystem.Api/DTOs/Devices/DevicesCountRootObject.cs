using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Devices
{
    public class DevicesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}