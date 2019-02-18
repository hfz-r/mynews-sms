using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Api.Models.ApiSettings
{
    public class ApiConfigurationModel
    {
        [Display(Name = "Enable Api")]
        public bool EnableApi { get; set; }
        public bool EnableApi_OverrideForStore { get; set; }

        [Display(Name = "Enable Logging")]
        public bool EnableLogging { get; set; }
        public bool EnableLogging_OverrideForStore { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }
    }
}