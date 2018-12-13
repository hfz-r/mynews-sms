namespace StockManagementSystem.Core.Domain.Identity
{
    public partial class RoleClaim : BaseEntity
    {
        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
    }
}
