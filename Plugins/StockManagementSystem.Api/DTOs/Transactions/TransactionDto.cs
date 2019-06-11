using System;
using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Transactions
{
    [JsonObject(Title = "transaction")]
    //TODO: TransactionDtoValidator
    public class TransactionDto : BaseDto
    {
        [JsonProperty("module_id")]
        public string ModuleId { get; set; }

        [JsonProperty("device_serial_no")]
        public string DeviceSerialNo { get; set; }

        [JsonProperty("branch_no")]
        public int? P_BranchNo { get; set; }

        [JsonProperty("staff_no")]
        public int? P_StaffNo { get; set; }

        [JsonProperty("stock_code")]
        public string P_StockCode { get; set; }

        [JsonProperty("qty")]
        public int? P_Qty { get; set; }

        [JsonProperty("unit_measurement_code")]
        public int? P_UnitMeasurementCode { get; set; }

        [JsonProperty("remark")]
        public string P_Remark { get; set; }

        [JsonProperty("resend")]
        public string P_Resend { get; set; }

        [JsonProperty("issref")]
        public string P_IssRef { get; set; }

        [JsonProperty("doc")]
        public string P_Doc { get; set; }

        [JsonProperty("ref")]
        public string P_Ref { get; set; }

        [JsonProperty("sysmod")]
        public string P_SysMod { get; set; }

        [JsonProperty("rectype")]
        public string P_RecType { get; set; }

        [JsonProperty("pcs_qty")]
        public int? PcsQty { get; set; }

        [JsonProperty("ctn_qty")]
        public int? CtnQty { get; set; }

        [JsonProperty("otr_qty")]
        public int? OtrQty { get; set; }

        [JsonProperty("is_port")]
        public bool? IsPost { get; set; }

        [JsonProperty("shift_no")]
        public string P_ShiftNo { get; set; }

        [JsonProperty("reason_code")]
        public string P_ReasonCode { get; set; }

        [JsonProperty("parent_id")]
        public int? P_ParentId { get; set; }

        [JsonProperty("group_id")]
        public int? P_GroupId { get; set; }

        [JsonProperty("unit_cost")]
        public double? P_UnitCost { get; set; }

        [JsonProperty("loc")]
        public string P_Loc { get; set; }

        [JsonProperty("sell_price")]
        public double? P_SellPrice { get; set; }

        [JsonProperty("cost")]
        public double? P_Cost { get; set; }

        [JsonProperty("desc")]
        public string P_Desc { get; set; }

        [JsonProperty("unit")]
        public int? P_Unit { get; set; }

        [JsonProperty("st_location")]
        public string STLocation { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("delivery_date")]
        public DateTime? DeliveryDate { get; set; }

        [JsonProperty("expiry_date")]
        public DateTime? ExpiryDate { get; set; }

        [JsonProperty("pos_doc")]
        public string POSDoc { get; set; }

        [JsonProperty("container_id")]
        public string ContainerId { get; set; }

        [JsonProperty("log_no")]
        public string LogNo { get; set; }
    }
}