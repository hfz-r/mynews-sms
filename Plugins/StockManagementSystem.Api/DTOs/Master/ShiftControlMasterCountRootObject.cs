using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class ShiftControlMasterCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
