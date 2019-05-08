using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class SalesMasterCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
