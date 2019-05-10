using Newtonsoft.Json;
using System;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Master
{
    [GeneratedController("api/master/sales")]
    [JsonObject(Title = "sales")]
    public class SalesMasterDto : BaseDto
    {
        [JsonProperty("stock_code")]
        public string P_StockCode { get; set; }

        [JsonProperty("branch_no")]
        public int P_BranchNo { get; set; }

        [JsonProperty("sales_qty")]
        public int P_SalesQty { get; set; }

        [JsonProperty("opening_balance_qty")]
        public int P_OpeningBalanceQty { get; set; }

        [JsonProperty("order_qty")]
        public int P_OrderQty { get; set; }

        [JsonProperty("receive_qty")]
        public int P_ReceiveQty { get; set; }

        [JsonProperty("return_qty")]
        public int P_ReturnQty { get; set; }

        [JsonProperty("txn_date")]
        public DateTime P_TxnDate { get; set; }

        [JsonProperty("status")]
        public byte Status { get; set; }
    }
}
