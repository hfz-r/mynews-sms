using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Master
{
    [GeneratedController("api/master/asndetail")]
    [JsonObject(Title = "asnDetail")]
    public class ASNDetailMasterDto : BaseDto
    {
        [JsonProperty("asn_no")]
        public string P_ASN_No { get; set; }

        [JsonProperty("branch_no")]
        public int P_BranchNo { get; set; }

        [JsonProperty("stock_code")]
        public string P_StockCode { get; set; }

        [JsonProperty("issue_ref")]
        public string P_IssueRef { get; set; }

        [JsonProperty("total_qty")]
        public int P_TotalQty { get; set; }

        [JsonProperty("container_id")]
        public string P_ContainerID { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
