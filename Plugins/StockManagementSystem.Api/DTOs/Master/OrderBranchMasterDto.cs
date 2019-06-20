using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Master
{
    [GeneratedController("api/master/orderbranch")]
    [JsonObject(Title = "orderBranch")]
    public class OrderBranchMasterDto : BaseDto
    {
        [JsonProperty("delivery_per_week")]
        public int P_DeliveryPerWeek { get; set; }

        [JsonProperty("safety")]
        public int P_Safety { get; set; }

        [JsonProperty("inventory_cycle")]
        public int P_InventoryCycle { get; set; }

        [JsonProperty("order_ratio")]
        public float P_OrderRatio { get; set; }

        [JsonProperty("min_days")]
        public int? P_MinDays { get; set; }

        [JsonProperty("max_days")]
        public int? P_MaxDays { get; set; }

        [JsonProperty("face_qty")]
        public int? P_FaceQty { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
