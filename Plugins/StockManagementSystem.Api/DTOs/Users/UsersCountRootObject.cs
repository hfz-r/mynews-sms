using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Users
{
    public class UsersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}