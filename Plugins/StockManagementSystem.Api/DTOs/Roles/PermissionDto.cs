using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Roles
{
    [GeneratedController("api/permissions")]
    [JsonObject(Title = "permission")]
    public class PermissionDto : BaseDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("system_name")]
        public string SystemName { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }
}