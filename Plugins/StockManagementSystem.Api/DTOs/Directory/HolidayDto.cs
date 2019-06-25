using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Directory
{
    [GeneratedController("api/holidays")]
    [JsonObject(Title = "holiday")]
    public class HolidayDto : BaseDto
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("day")]
        public string Day { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }
}