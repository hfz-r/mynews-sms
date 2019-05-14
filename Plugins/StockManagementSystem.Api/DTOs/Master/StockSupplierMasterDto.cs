using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Master
{
    [GeneratedController("api/master/stocksupplier")]
    [JsonObject(Title = "stockSupplier")]
    public class StockSupplierMasterDto : BaseDto
    {
        [JsonProperty("stock_code")]
        public string P_StockCode { get; set; }

        [JsonProperty("supplier_no")]
        public int P_SupplierNo { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
