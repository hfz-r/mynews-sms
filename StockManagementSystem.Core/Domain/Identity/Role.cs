using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StockManagementSystem.Core.Domain.Security;

namespace StockManagementSystem.Core.Domain.Identity
{
    public partial class Role : Entity
    {
        private ICollection<PermissionRoles> _permissionRoles;

        public string ConcurrencyStamp { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public string SystemName { get; set; }

        [MaxLength(Int32.MaxValue)]
        public string Description { get; set; }

        public virtual ICollection<RoleClaim> Claims { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }

        public virtual ICollection<PermissionRoles> PermissionRoles
        {
            get => _permissionRoles ?? (_permissionRoles = new List<PermissionRoles>());
            protected set => _permissionRoles = value;
        }
    }
}
