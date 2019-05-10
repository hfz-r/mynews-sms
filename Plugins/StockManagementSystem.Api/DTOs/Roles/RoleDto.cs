using System.Collections.Generic;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using StockManagementSystem.Api.Validators;

namespace StockManagementSystem.Api.DTOs.Roles
{
    [JsonObject(Title = "role")]
    [Validator(typeof(RoleDtoValidator))]
    public class RoleDto : BaseDto
    {
        private List<int> _permissionIds;
        private List<PermissionDto> _permissions;

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("active")]
        public bool? Active { get; set; }

        [JsonProperty("is_system_role")]
        public bool? IsSystemRole { get; set; }

        [JsonProperty("system_name")]
        public string SystemName { get; set; }

        [JsonIgnore]
        [JsonProperty("permission_ids")]
        public List<int> PermissionIds
        {
            get => _permissionIds ?? (_permissionIds = new List<int>());
            set => _permissionIds = value;
        }

        [JsonProperty("permissions")]
        public List<PermissionDto> Permissions
        {
            get => _permissions ?? (_permissions = new List<PermissionDto>());
            set => _permissions = value;
        }
    }
}