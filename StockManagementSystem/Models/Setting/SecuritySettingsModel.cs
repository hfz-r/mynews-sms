using System.ComponentModel.DataAnnotations;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Setting
{
    public class SecuritySettingsModel : BaseModel, ISettingsModel
    {
        public int ActiveTenantScopeConfiguration { get; set; }

        [Display(Name = "Encryption private key")] 
        public string EncryptionKey { get; set; }

        [Display(Name = "Allowed IP")]
        public string AllowedIpAddresses { get; set; }

        [Display(Name = "Force SSL for all site pages")]
        public bool ForceSslForAllPages { get; set; }

        [Display(Name = "Enable XSRF protection")]
        public bool EnableXsrfProtection { get; set; }
    }
}