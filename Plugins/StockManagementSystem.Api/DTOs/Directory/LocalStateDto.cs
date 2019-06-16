using System.Collections.Generic;
using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Directory
{
    [GeneratedController("api/states")]
    [JsonObject(Title = "state")]
    public class LocalStateDto : BaseDto
    {
        private List<HolidayDto> _holidays;

        [JsonProperty("abbreviation")]
        public string Abbreviation { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("is_same_weekend")]
        public bool? IsSameWeekend { get; set; }

        [JsonProperty("holidays")]
        public List<HolidayDto> Holidays
        {
            get => _holidays ?? (_holidays = new List<HolidayDto>());
            set => _holidays = value;
        }
    }
}