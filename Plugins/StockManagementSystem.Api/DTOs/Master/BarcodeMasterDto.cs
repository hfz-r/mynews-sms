using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Master
{
    [GeneratedController("api/master/barcode")]
    [JsonObject(Title = "barcode")]
    public class BarcodeMasterDto : BaseDto
    {
        [JsonProperty("barcode")]
        public string P_Barcode { get; set; }

        [JsonProperty("stock_code")]
        public string P_StockCode { get; set; }

        [JsonProperty("um_id")]
        public int P_UM_ID { get; set; }

        [JsonProperty("um_qty")]
        public double P_UM_Qty { get; set; }

        [JsonProperty("priority_id")]
        public int P_PriorityID { get; set; }

        [JsonProperty("issue_ref")]
        public string P_IssueRef { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
