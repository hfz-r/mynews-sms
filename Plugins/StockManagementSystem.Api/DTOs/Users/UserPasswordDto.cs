using System;
using Newtonsoft.Json;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Api.DTOs.Users
{
    [JsonObject(Title = "user_password")]
    public class UserPasswordDto : BaseDto
    {
        [JsonIgnore]
        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonIgnore]
        [JsonProperty("password_format_id")]
        public int PasswordFormatId { get; set; }

        [JsonIgnore]
        [JsonProperty("password_salt")]
        public string PasswordSalt { get; set; }

        [JsonProperty("created_on")]
        public DateTime CreatedOnUtc { get; set; }

        [JsonProperty("password_format")]
        public string PasswordFormat
        {
            get => ((PasswordFormat)this.PasswordFormatId).ToString();
            set
            {
                if (Enum.TryParse(value, out PasswordFormat passwordFormat))
                    this.PasswordFormatId = (int) passwordFormat;
            }
        }
    }
}