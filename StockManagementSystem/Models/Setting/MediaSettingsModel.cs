using System.ComponentModel.DataAnnotations;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Setting
{
    public class MediaSettingsModel : BaseModel, ISettingsModel
    {
        public int ActiveTenantScopeConfiguration { get; set; }

        [Display(Name = "Pictures are stored into?")]
        public bool PicturesStoredIntoDatabase { get; set; }

        [Display(Name = "Avatar image size")]
        public int AvatarPictureSize { get; set; }
        public bool AvatarPictureSize_OverrideForTenant { get; set; }

        [Display(Name = "Maximum image size")]
        public int MaximumImageSize { get; set; }
        public bool MaximumImageSize_OverrideForTenant { get; set; }

        [Display(Name = "Default image quality (0 - 100)")]
        public int DefaultImageQuality { get; set; }
        public bool DefaultImageQuality_OverrideForTenant { get; set; }
    }
}