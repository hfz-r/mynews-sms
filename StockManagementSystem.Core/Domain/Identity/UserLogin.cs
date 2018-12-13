namespace StockManagementSystem.Core.Domain.Identity
{
    public partial class UserLogin : BaseEntity
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public string ProviderDisplayName { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
