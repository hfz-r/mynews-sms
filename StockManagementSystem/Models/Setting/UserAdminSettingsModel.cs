using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Setting
{
    public class UserAdminSettingsModel : BaseModel, ISettingsModel
    {
        public UserAdminSettingsModel()
        {
            UserSettings = new UserSettingsModel();
            DateTimeSettings = new DateTimeSettingsModel();
        }

        public int ActiveTenantScopeConfiguration { get; set; }

        public UserSettingsModel UserSettings { get; set; }

        public DateTimeSettingsModel DateTimeSettings { get; set; }
    }
}