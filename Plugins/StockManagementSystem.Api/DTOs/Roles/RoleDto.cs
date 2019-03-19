using FluentValidation.Attributes;
using Newtonsoft.Json;
using StockManagementSystem.Api.Validators;

namespace StockManagementSystem.Api.DTOs.Roles
{
    [JsonObject(Title = "role")]
    [Validator(typeof(RoleDtoValidator))]
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