using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Roles
{
    [GeneratedController("api/permission_roles")]
    [JsonObject(Title = "permission_roles")]
    public class PermissionRolesDto : BaseDto
    {
        [JsonProperty("permission")]
        public PermissionDto PermissionDto { get; set; }

        [JsonProperty("role")]
        public RoleDto RoleDto { get; set; }
    }
}