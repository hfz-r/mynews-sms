using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.DTOs.Users;

namespace StockManagementSystem.Api.DTOs.Roles
{
    [GeneratedController("api/user_role")]
    [JsonObject(Title = "user_role")]
    public class UserRoleDto : BaseDto
    {
        [JsonProperty("user")]
        public UserDto UserDto { get; set; }

        [JsonProperty("role")]
        public RoleDto RoleDto { get; set; }
    }
}