using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Master
{
    [GeneratedController("api/master/branch")]
    [JsonObject(Title = "branch")]
    public class BranchMasterDto : BaseDto
    {
        [JsonProperty("branch_no")]
        public int P_BranchNo { get; set; }

        [JsonProperty("name")]
        public string P_Name { get; set; }

        [JsonProperty("area_code")]
        public string P_AreaCode { get; set; }

        [JsonProperty("addr1")]
        public string P_Addr1 { get; set; }

        [JsonProperty("addr2")]
        public string P_Addr2 { get; set; }

        [JsonProperty("addr3")]
        public string P_Addr3 { get; set; }

        [JsonProperty("state")]
        public string P_State { get; set; }

        [JsonProperty("city")]
        public string P_City { get; set; }

        [JsonProperty("country")]
        public string P_Country { get; set; }

        [JsonProperty("postcode")]
        public int P_Postcode { get; set; }

        [JsonProperty("price_level")]
        public int P_PriceLevel { get; set; }

        [JsonProperty("latitude")]
        public float Latitude { get; set; }

        [JsonProperty("longitude")]
        public float Longitude { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }      
    }
}