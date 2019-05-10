using Newtonsoft.Json;
using System;

namespace StockManagementSystem.Api.DTOs.Master
{
    [JsonObject(Title = "stockTakeControlOutlet")]
    public class StockTakeControlOutletMasterDto : BaseDto
    {
        [JsonProperty("stock_take_no")]
        public int P_StockTakeNo { get; set; }

        [JsonProperty("branch_no")]
        public int P_BranchNo { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
