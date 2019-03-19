using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Common
{
    public class HeaderLinksModel : BaseModel
    {
        public bool IsAuthenticated { get; set; }

        public string UserName { get; set; }
    }
}