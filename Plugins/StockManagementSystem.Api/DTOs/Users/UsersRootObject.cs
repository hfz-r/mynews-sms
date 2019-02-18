using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Users
{
    public class UsersRootObject : ISerializableObject
    {
        public UsersRootObject()
        {
            Users = new List<UserDto>();
        }

        [JsonProperty("users")]
        public IList<UserDto> Users { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "users";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(UserDto);
        }
    }
}