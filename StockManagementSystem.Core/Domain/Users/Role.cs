using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Security;

namespace StockManagementSystem.Core.Domain.Users
{
    public class Role : BaseEntity
    {
        private ICollection<PermissionRoles> _permissionRoles;

        public string Name { get; set; }

        public bool Active { get; set; }

        public bool IsSystemRole { get; set; }

        public string SystemName { get; set; }

        public bool EnablePasswordLifetime { get; set; }

        public virtual ICollection<PermissionRoles> PermissionRoles
        {
            get => _permissionRoles ?? (_permissionRoles = new List<PermissionRoles>());
            protected set => _permissionRoles = value;
        }
    }
}