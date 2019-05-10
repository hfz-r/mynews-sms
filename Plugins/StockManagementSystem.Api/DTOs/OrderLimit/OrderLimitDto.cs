using System.Collections.Generic;
using Newtonsoft.Json;
using StockManagementSystem.Api.DTOs.Stores;

namespace StockManagementSystem.Api.DTOs.OrderLimit
{
    [JsonObject(Title = "order_limit")]
    //TODO: OrderLimitDtoValidator
    public class OrderLimitDto : BaseDto
    {
        private List<int> _storeIds;
        private List<StoreDto> _stores;

        [JsonProperty("delivery_per_week")]
        public int DeliveryPerWeek { get; set; }

        [JsonProperty("safety")]
        public int Safety { get; set; }

        [JsonProperty("inventory_cycle")]
        public int InventoryCycle { get; set; }

        [JsonProperty("order_ratio")]
        public int OrderRatio { get; set; }

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