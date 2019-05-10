using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Master
{
    [GeneratedController("api/master/shiftcontrol")]
    [JsonObject(Title = "shiftControl")]
    public class ShiftControlMasterDto : BaseDto
    {
        [JsonProperty("stock_code")]
        public string P_StockCode { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
