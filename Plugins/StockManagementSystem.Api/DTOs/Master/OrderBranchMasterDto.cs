using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    [JsonObject(Title = "orderBranch")]
    public class OrderBranchMasterDto : BaseDto
    {
        [JsonProperty("branch_no")]
        public int P_BranchNo { get; set; }

        [JsonProperty("delivery_per_week")]
        public int P_DeliveryPerWeek { get; set; }

        [JsonProperty("safety")]
        public int P_Safety { get; set; }

        [JsonProperty("inventory_cycle")]
        public int P_InventoryCycle { get; set; }

        [JsonProperty("order_ratio")]
        public int P_OrderRatio { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
