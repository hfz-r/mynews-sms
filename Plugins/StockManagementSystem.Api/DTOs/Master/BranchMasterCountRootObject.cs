using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class BranchMasterCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
