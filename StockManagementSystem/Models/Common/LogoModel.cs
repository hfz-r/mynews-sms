using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Common
{
    public class LogoModel : BaseModel
    {
        public string TenantName { get; set; }

        public string LogoPath { get; set; }
    }
}