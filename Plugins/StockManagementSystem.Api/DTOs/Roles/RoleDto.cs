using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Roles
{
    [JsonObject(Title = "role")]
    public class RoleDto : BaseDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("active")]
        public bool? Active { get; set; }

        [JsonProperty("is_system_role")]
        public bool? IsSystemRole { get; set; }

        [JsonProperty("system_name")]
        public string SystemName { get; set; }
    }
}