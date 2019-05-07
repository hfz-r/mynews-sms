using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class OrderBranchMasterCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
