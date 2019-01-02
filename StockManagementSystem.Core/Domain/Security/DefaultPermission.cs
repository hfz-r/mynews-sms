using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Security
{
    public class DefaultPermission
    {
        public DefaultPermission()
        {
            this.Permissions = new List<Permission>();
        }

        public string RoleSystemName { get; set; }

        public IEnumerable<Permission> Permissions { get; set; }
    }
}