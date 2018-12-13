namespace StockManagementSystem.Core.Domain.Identity
{
    public partial class UserToken : BaseEntity
    {
        public string LoginProvider { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
