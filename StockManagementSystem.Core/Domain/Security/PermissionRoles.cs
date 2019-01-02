using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Core.Domain.Security
{
    /// <summary>
    /// Represents a permission-user role mapping class
    /// </summary>
    public class PermissionRoles : BaseEntity
    {
        public int PermissionId { get; set; }

        public int RoleId { get; set; }

        public virtual Permission Permission { get; set; }

        public virtual Role Role { get; set; }
    }
}