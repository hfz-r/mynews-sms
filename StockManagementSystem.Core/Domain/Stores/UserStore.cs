using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Core.Domain.Stores
{
    /// <summary>
    /// Represents a store-user mapping class
    /// </summary>
    public class UserStore : BaseEntity
    {
        /// <summary>
        /// This will represent by the BranchNo
        /// </summary>
        public int StoreId { get; set; }

        public int UserId { get; set; }

        public virtual Store Store { get; set; }

        public virtual User User { get; set; }
    }
}