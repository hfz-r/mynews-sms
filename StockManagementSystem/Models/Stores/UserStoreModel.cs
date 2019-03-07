using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Stores
{
    public class UserStoreModel : BaseEntityModel
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public bool Active { get; set; }
    }
}