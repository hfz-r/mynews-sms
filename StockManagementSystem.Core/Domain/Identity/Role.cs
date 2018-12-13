using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Core.Domain.Identity
{
    public partial class Role : BaseEntity
    {
        public string ConcurrencyStamp { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        [MaxLength(Int32.MaxValue)]
        public string Description { get; set; }

        public virtual ICollection<RoleClaim> Claims { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
