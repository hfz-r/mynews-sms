using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Master
{
    [GeneratedController("api/master/warehousedeliveryschedule")]
    [JsonObject(Title = "warehouseDeliverySchedule")]
    public class WarehouseDeliveryScheduleMasterDto : BaseDto
    {
        [JsonProperty("branch_no")]
        public int P_BranchNo { get; set; }

        [JsonProperty("day1")]
        public byte P_Day1 { get; set; }

        [JsonProperty("day2")]
        public byte P_Day2 { get; set; }

        [JsonProperty("day3")]
        public byte P_Day3 { get; set; }

        [JsonProperty("day4")]
        public byte P_Day4 { get; set; }

        [JsonProperty("day5")]
        public byte P_Day5 { get; set; }

        [JsonProperty("day6")]
        public byte P_Day6 { get; set; }

        [JsonProperty("day7")]
        public byte P_Day7 { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
