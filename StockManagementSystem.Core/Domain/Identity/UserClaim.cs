namespace StockManagementSystem.Core.Domain.Identity
{
    public partial class UserClaim : BaseEntity
    {
        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
