using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Roles
{
    [JsonObject(Title = "permission")]
    //TODO: PermissionDtoValidator
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