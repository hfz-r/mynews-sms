using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class ASNHeaderMasterCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
