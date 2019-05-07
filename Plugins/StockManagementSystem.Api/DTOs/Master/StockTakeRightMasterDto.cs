using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    [JsonObject(Title = "stockTakeRight")]
    public class StockTakeRightMasterDto : BaseDto
    {
        [JsonProperty("role")]
        public string P_Role { get; set; }

        [JsonProperty("stock_take_no")]
        public int P_StockTakeNo { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
