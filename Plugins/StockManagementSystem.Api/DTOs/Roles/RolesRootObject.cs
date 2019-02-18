using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Roles
{
    public class RolesRootObject : ISerializableObject
    {
        public RolesRootObject()
        {
            Roles = new List<RoleDto>();    
        }

        [JsonProperty("roles")]
        public IList<RoleDto> Roles { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "roles";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(RoleDto);
        }
    }
}