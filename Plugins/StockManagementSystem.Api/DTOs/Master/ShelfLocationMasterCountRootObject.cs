using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class ShelfLocationMasterCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
