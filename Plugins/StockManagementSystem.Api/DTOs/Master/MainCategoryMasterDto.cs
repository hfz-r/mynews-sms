using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    [JsonObject(Title = "mainCategory")]
    public class MainCategoryMasterDto : BaseDto
    {
        [JsonProperty("code")]
        public int P_Code { get; set; }

        [JsonProperty("name")]
        public string P_Name { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
