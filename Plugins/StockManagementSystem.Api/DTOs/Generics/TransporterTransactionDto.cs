using Newtonsoft.Json;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Generics
{
    [GeneratedController("api/transporter")]
    [JsonObject(Title = "transporter_transaction")]
    public class TransporterTransactionDto : BaseDto
    {
        [JsonProperty("driver_name")]
        public string DriverName { get; set; }

        [JsonProperty("vehicle_plate_no")]
        public string VehiclePlateNo { get; set; }

        [JsonProperty("doc_no")]
        public string DocNo { get; set; }

        [JsonProperty("container_id")]
        public int? ContainerId { get; set; }

        [JsonProperty("module_code")]
        public string ModuleCode { get; set; }

        [JsonProperty("qty")]
        public int? Qty { get; set; }
    }
}