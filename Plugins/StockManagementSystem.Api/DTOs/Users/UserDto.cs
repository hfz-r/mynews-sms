using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using StockManagementSystem.Api.Validators;

namespace StockManagementSystem.Api.DTOs.Users
{
    [JsonObject(Title = "user")]
    [Validator(typeof(UserDtoValidator))]
    public class UserDto : BaseDto
    {
        private List<int> _roleIds;

        [JsonProperty("user_guid")]
        public Guid UserGuid { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonIgnore]
        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("admin_comment")]
        public string AdminComment { get; set; }

        [JsonProperty("active")]
        public bool? Active { get; set; }

        [JsonProperty("deleted")]
        public bool? Deleted { get; set; }

        [JsonProperty("is_system_account")]
        public bool? IsSystemAccount { get; set; }

        [JsonProperty("system_name")]
        public string SystemName { get; set; }

        [JsonProperty("last_ip_address")]
        public string LastIpAddress { get; set; }

        [JsonProperty("created_on_utc")]
        public DateTime? CreatedOnUtc { get; set; }

        [JsonProperty("last_login_date_utc")]
        public DateTime? LastLoginDateUtc { get; set; }

        [JsonProperty("last_activity_date_utc")]
        public DateTime? LastActivityDateUtc { get; set; }

        [JsonProperty("registered_in_tenant_id")]
        public int? RegisteredInTenantId { get; set; }

        [JsonProperty("role_ids")]
        public List<int> RoleIds
        {
            get => _roleIds ?? (_roleIds = new List<int>());
            set => _roleIds = value;
        }
    }
}