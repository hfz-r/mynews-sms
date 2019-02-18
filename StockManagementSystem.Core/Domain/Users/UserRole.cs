namespace StockManagementSystem.Core.Domain.Users
{
    /// <summary>
    /// Represents a user-role mapping class
    /// </summary>
    public class UserRole : BaseEntity
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
}