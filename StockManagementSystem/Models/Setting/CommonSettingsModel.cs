using System.ComponentModel.DataAnnotations;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Setting
{
    public class CommonSettingsModel : BaseModel, ISettingsModel 
    {
        public int ActiveTenantScopeConfiguration { get; set; }

        [UIHint("Picture")]
        [Display(Name = "Logo")]
        public int LogoPictureId { get; set; }
        public bool LogoPictureId_OverrideForTenant { get; set; }

        [Display(Name = "Use response compression")]
        public bool UseResponseCompression { get; set; }
        public bool UseResponseCompression_OverrideForTenant { get; set; }
    }
}