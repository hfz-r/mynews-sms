using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Common
{
    public class AdminHeaderLinksModel : BaseModel
    {
        public bool DisplayAdminLink { get; set; }

        public string EditPageUrl { get; set; }
    }
}