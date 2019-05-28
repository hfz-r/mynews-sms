using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Transactions
{
    public class TransactionsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}