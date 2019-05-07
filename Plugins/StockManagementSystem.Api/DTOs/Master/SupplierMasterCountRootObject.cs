using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class SupplierMasterCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
