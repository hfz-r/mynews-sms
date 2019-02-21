using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Api.Models.ApiSettings
{
    public class ApiConfigurationModel
    {
        [Display(Name = "Enable Api")]
        public bool EnableApi { get; set; }
        public bool EnableApi_OverrideForTenant { get; set; }

        [Display(Name = "Enable Logging")]
        public bool EnableLogging { get; set; }
        public bool EnableLogging_OverrideForTenant { get; set; }

        public int ActiveTenantScopeConfiguration { get; set; }
    }
}