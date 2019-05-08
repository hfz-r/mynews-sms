using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Roles
{
    public class RolesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}