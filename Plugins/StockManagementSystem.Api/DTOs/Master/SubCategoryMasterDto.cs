using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Master
{
    [JsonObject(Title = "subCategory")]
    public class SubCategoryMasterDto : BaseDto
    {
        [JsonProperty("code")]
        public int P_Code { get; set; }

        [JsonProperty("name")]
        public string P_Name { get; set; }

        [JsonProperty("main_category_id")]
        public int P_MainCategoryID { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
