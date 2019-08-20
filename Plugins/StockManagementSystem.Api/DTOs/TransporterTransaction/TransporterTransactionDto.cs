using System;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using StockManagementSystem.Api.Validators;

namespace StockManagementSystem.Api.DTOs.TransporterTransaction
{
    [JsonObject(Title = "transporter_transaction")]
    [Validator(typeof(TransporterDtoValidator))]
    public class TransporterTransactionDto : BaseDto
    {
        [JsonProperty("driver_name")]
        public string DriverName { get; set; }

        [JsonProperty("vehicle_plate_no")]
        public string VehiclePlateNo { get; set; }

        [JsonProperty("doc_no")]
        public string DocNo { get; set; }

        [JsonProperty("container_id")]
        public string ContainerId { get; set; }

        [JsonProperty("module_code")]
        public string ModuleCode { get; set; }

        [JsonProperty("qty")]
        public int? Qty { get; set; }

        [JsonProperty("branch_no")]
        public int BranchNo { get; set; }

        [JsonProperty("created_on_utc")]
        public DateTime? CreatedOnUtc { get; set; }

        [JsonProperty("staff_no")]
        public int? P_StaffNo { get; set; }

        [JsonProperty("sysmod")]
        public string P_SysMod { get; set; }
    }
}