using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Security
{
    public partial class Permission : BaseEntity
    {
        private ICollection<PermissionRoles> _permissionRoles;

        public string Name { get; set; }

        public string SystemName { get; set; }

        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the permission-user role mappings
        /// </summary>
        public virtual ICollection<PermissionRoles> PermissionRoles
        {
            get => _permissionRoles ?? (_permissionRoles = new List<PermissionRoles>());
            set => _permissionRoles = value;
        }
    }
}