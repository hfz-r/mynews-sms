using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.DTOs.Users;

namespace StockManagementSystem.Api.DTOs.Stores
{
    [GeneratedController("api/user_store")]
    [JsonObject(Title = "user_store")]
    public class UserStoreDto : BaseDto
    {
        [JsonProperty("store")]
        public StoreDto StoreDto { get; set; }

        [JsonProperty("user")]
        public UserDto UserDto { get; set; }
    }
}