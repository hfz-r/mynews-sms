using Newtonsoft.Json;

namespace StockManagementSystem.Api.Models.ItemsParameters
{
    public class BaseItemsParametersModel
    {
        /// <summary>
        /// Show all the group-item mappings for this item
        /// </summary>
        [JsonProperty("group_id")]
        public int? GroupId { get; set; }

        /// <summary>
        /// Show all the vendor-item mappings for this item
        /// </summary>
        [JsonProperty("vendor_id")]
        public int? VendorId { get; set; }
    }
}