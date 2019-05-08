using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class SubCategoryMasterCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
