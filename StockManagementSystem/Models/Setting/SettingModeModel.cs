using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Setting
{
    public class SettingModeModel : BaseModel
    {
        public string ModeName { get; set; }

        public bool Enabled { get; set; }
    }
}