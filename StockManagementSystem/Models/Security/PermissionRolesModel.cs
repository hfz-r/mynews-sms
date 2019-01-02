using System.Collections.Generic;
using StockManagementSystem.Models.Roles;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Security
{
    public class PermissionRolesModel : BaseModel
    {
        public PermissionRolesModel()
        {
            AvailablePermissions = new List<PermissionModel>();
            AvailableRoles = new List<RoleModel>();
            Allowed = new Dictionary<string, IDictionary<int, bool>>();
        }

        public IList<PermissionModel> AvailablePermissions { get; set; }

        public IList<RoleModel> AvailableRoles { get; set; }

        //[permission system name] / [role id] / [allowed]
        public IDictionary<string, IDictionary<int, bool>> Allowed { get; set; }
    }
}