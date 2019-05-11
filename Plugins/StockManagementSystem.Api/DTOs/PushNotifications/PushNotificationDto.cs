using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using StockManagementSystem.Api.DTOs.Generics;

namespace StockManagementSystem.Api.DTOs.PushNotifications
{
    [JsonObject(Title = "push_notification")]
    //TODO: PushNotificationDtoValidator
    public class PushNotificationDto : BaseDto
    {
        private List<int> _storeIds;
        private List<StoreDto> _stores;

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("stock_take_no")]
        public string StockTakeNo { get; set; }

        [JsonProperty("notification_category_id")]
        public int NotificationCategoryId { get; set; }

        [JsonProperty("job_name")]
        public string JobName { get; set; }

        [JsonProperty("job_group")]
        public string JobGroup { get; set; }

        [JsonProperty("interval")]
        public int? Interval { get; set; }

        [JsonProperty("remind_me")]
        public bool RemindMe { get; set; }

        [JsonProperty("start_time")]
        public DateTime? StartTime { get; set; }

        [JsonProperty("end_time")]
        public DateTime? EndTime { get; set; }

        [JsonIgnore]
        [JsonProperty("store_ids")]
        public List<int> StoreIds
        {
            get => _storeIds ?? (_storeIds = new List<int>());
            set => _storeIds = value;
        }

        [JsonProperty("stores")]
        public List<StoreDto> Stores
        {
            get => _stores ?? (_stores = new List<StoreDto>());
            set => _stores = value;
        }
    }
}