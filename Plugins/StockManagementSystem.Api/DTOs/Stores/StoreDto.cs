using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Stores
{
    [GeneratedController("api/stores")]
    [JsonObject(Title = "store")]
    public class StoreDto : BaseDto
    {
        [JsonProperty("branch_no")]
        public int BranchNo { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("area_code")]
        public string AreaCode { get; set; }

        [JsonProperty("address_1")]
        public string Address1 { get; set; }

        [JsonProperty("address_2")]
        public string Address2 { get; set; }

        [JsonProperty("address_3")]
        public string Address3 { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("county")]
        public string Country { get; set; }

        [JsonProperty("postcode")]
        public string Postcode { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("price_level")]
        public int PriceLevel { get; set; }

        [JsonProperty("db_ipaddress")]
        public string DBIPAddress { get; set; }

        [JsonProperty("db_name")]
        public string DBName { get; set; }

        [JsonProperty("db_username")]
        public string DBUsername { get; set; }

        [JsonProperty("db_password")]
        public string DBPassword { get; set; }
    }
}