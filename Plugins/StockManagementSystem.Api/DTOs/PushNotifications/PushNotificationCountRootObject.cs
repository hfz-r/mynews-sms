using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.PushNotifications
{
    public class PushNotificationCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}