using System;
using System.Collections.Generic;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using StockManagementSystem.Api.DTOs.Roles;
using StockManagementSystem.Api.DTOs.Stores;
using StockManagementSystem.Api.Validators;

namespace StockManagementSystem.Api.DTOs.Users
{
    [JsonObject(Title = "user")]
    [Validator(typeof(UserDtoValidator))]
    public class UserDto : BaseDto
    {
        private List<int> _roleIds;
        private List<int> _storeIds;
        private List<RoleDto> _roles;
        private List<StoreDto> _stores;

        [JsonProperty("user_guid")]
        public Guid UserGuid { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonIgnore]
        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("login_token")]
        public bool LoginToken { get; set; }//for HHT 

        [JsonProperty("password_last_modified")]
        public DateTime? PasswordLastModified { get; set; } //for HHT 

        [JsonProperty("user_password")]
        public UserPasswordDto UserPassword { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

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

        [JsonIgnore]
        [JsonProperty("registered_in_tenant_id")]
        public int? RegisteredInTenantId { get; set; }

        [JsonIgnore]
        [JsonProperty("role_ids")]
        public List<int> RoleIds
        {
            get => _roleIds ?? (_roleIds = new List<int>());
            set => _roleIds = value;
        }

        [JsonIgnore]
        [JsonProperty("store_ids")]
        public List<int> StoreIds
        {
            get => _storeIds ?? (_storeIds = new List<int>());
            set => _storeIds = value; 
        }

        [JsonProperty("roles")]
        public List<RoleDto> Roles
        {
            get => _roles ?? (_roles = new List<RoleDto>());
            set => _roles = value;
        }

        [JsonProperty("stores")]
        public List<StoreDto> Stores
        {
            get => _stores ?? (_stores = new List<StoreDto>());
            set => _stores = value;
        }
    }
}