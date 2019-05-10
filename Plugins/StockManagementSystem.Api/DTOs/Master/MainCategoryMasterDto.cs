using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Master
{
    [GeneratedController("api/master/maincategory")]
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
