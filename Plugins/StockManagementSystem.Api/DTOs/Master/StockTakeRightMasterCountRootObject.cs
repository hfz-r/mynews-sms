using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class StockTakeRightMasterCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
