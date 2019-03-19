using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Setting
{
    public class GeneralCommonSettingsModel : BaseModel, ISettingsModel
    {
        public GeneralCommonSettingsModel()
        {
            CommonSettings = new CommonSettingsModel();
            SecuritySettings = new SecuritySettingsModel();
        }

        public int ActiveTenantScopeConfiguration { get; set; }

        public CommonSettingsModel CommonSettings { get; set; }

        public SecuritySettingsModel SecuritySettings { get; set; }
    }
}