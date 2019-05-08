using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Generics
{
    public class GenericCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}