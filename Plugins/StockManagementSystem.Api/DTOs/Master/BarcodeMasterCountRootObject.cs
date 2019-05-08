using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class BarcodeMasterCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
