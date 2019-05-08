using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Master
{
    [GeneratedController("api/master/stocktakeright")]
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
