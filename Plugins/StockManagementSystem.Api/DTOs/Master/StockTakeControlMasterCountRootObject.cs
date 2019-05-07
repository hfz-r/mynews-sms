using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class StockTakeControlMasterCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
