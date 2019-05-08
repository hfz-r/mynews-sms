using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Stores
{
    [JsonObject(Title = "store")]
    //TODO: StoreDtoValidator
    public class StoreDto : BaseDto
    {
        [JsonProperty("branch_no")]
        public int BranchNo { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rec_status")]
        public string RecStatus { get; set; }

        [JsonProperty("comp_id")]
        public string CompId { get; set; }

        [JsonProperty("sell_price_level")]
        public string SellPriceLevel { get; set; }

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
        public string PostCode { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }
    }
}