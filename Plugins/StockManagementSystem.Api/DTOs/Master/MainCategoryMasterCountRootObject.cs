using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class MainCategoryMasterCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
