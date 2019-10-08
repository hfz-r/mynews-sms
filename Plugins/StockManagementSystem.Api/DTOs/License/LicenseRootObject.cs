using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.License
{
    public class LicenseRootObject
    {
        [JsonProperty("valid")]
        public bool IsValid { get; set; }
    }
}