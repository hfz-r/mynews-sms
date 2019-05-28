using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.TransporterTransaction
{
    public class TransporterTransactionCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}