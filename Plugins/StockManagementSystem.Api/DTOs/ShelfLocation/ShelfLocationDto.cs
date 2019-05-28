using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.ShelfLocation
{
    [JsonObject(Title = "shelf_location")]
    //TODO: ShelfLocationDtoValidator
    public class ShelfLocationDto : BaseDto
    {
        [JsonIgnore]
        [JsonProperty("store_id")]
        public int StoreId { get; set; }

        [JsonIgnore]
        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("stock_code")]
        public string P_StockCode { get; set; }

        [JsonProperty("branch_no")]
        public int? P_BranchNo { get; set; }

        [JsonProperty("gondola")]
        public string P_Gondola { get; set; }

        [JsonProperty("row")]
        public string P_Row { get; set; }

        [JsonProperty("face")]
        public string P_Face { get; set; }

        [JsonProperty("post")]
        public bool? IsPost { get; set; }

        [JsonProperty("issue_ref")]
        public string IssueRef { get; set; }
    }
}