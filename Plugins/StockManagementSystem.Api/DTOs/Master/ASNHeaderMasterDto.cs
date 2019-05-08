using Newtonsoft.Json;
using System;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Master
{
    [GeneratedController("api/master/asnheader")]
    [JsonObject(Title = "asnHeader")]
    public class ASNHeaderMasterDto : BaseDto
    {
        [JsonProperty("asn_no")]
        public string P_ASN_No { get; set; }

        [JsonProperty("delivery_date")]
        public DateTime P_DeliveryDate { get; set; }

        [JsonProperty("branch_no")]
        public int P_BranchNo { get; set; }

        [JsonProperty("supplier_no")]
        public int P_SupplierNo { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
