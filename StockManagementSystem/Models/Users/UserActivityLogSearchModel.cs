using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Users
{
    public class UserActivityLogSearchModel : BaseSearchModel
    {
        public int UserId { get; set; }
    }
}