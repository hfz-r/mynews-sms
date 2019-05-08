using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.PushNotifications
{
    public class PushNotificationRootObject : ISerializableObject
    {
        public PushNotificationRootObject()
        {
            PushNotifications = new List<PushNotificationDto>();
        }

        [JsonProperty("push_notifications")]
        public IList<PushNotificationDto> PushNotifications { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "push_notifications";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(PushNotificationDto);
        }
    }
}