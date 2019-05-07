using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class ASNDetailMasterCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
