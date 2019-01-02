using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Core.Domain.Security
{
    public partial class AclRecord : BaseEntity
    {
        public int EntityId { get; set; }

        public string EntityName { get; set; }

        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
    }
}