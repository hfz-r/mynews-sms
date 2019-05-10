using Newtonsoft.Json;
using System;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Master
{
    [GeneratedController("api/master/stocktakecontrol")]
    [JsonObject(Title = "stockTakeControl")]
    public class StockTakeControlMasterDto : BaseDto
    {
        [JsonProperty("stock_take_no")]
        public int P_StockTakeNo { get; set; }

        [JsonProperty("branch_no")]
        public int P_BranchNo { get; set; }

        [JsonProperty("begin_date")]
        public DateTime P_BeginDate { get; set; }

        [JsonProperty("end_date")]
        public DateTime P_EndDate { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
