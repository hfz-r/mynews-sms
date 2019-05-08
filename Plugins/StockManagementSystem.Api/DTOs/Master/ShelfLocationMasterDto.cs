using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Master
{
    [GeneratedController("api/master/shelflocation")]
    [JsonObject(Title = "shelfLocation")]
    public class ShelfLocationMasterDto : BaseDto
    {
        [JsonProperty("stock_code")]
        public string P_StockCode { get; set; }

        [JsonProperty("branch_no")]
        public int P_BranchNo { get; set; }

        [JsonProperty("gondola")]
        public string P_Gondola { get; set; }

        [JsonProperty("shelf_row")]
        public string P_ShelfRow { get; set; }

        [JsonProperty("horizontal_facing")]
        public int P_HorizontalFacing { get; set; }

        [JsonProperty("vertical_facing")]
        public int P_VerticalFacing { get; set; }

        [JsonProperty("depth_facing")]
        public int P_DepthFacing { get; set; }

        [JsonProperty("total_display")]
        public int P_TotalDisplay { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
