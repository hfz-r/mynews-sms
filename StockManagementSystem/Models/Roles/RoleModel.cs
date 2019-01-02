using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Roles
{
    public class RoleModel : BaseEntityModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}