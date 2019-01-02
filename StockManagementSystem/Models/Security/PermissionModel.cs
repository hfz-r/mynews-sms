using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Security
{
    public class PermissionModel : BaseModel
    {
        public string Name { get; set; }

        public string SystemName { get; set; }
    }
}