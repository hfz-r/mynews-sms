using System;

namespace StockManagementSystem.Core.Domain.Transactions
{
    /// <summary>
    /// Represent fake transaction entity 
    /// </summary>
    public class Transaction : BaseEntity
    {
        public string Category { get; set; }

        public int BranchId { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public virtual Branch Branch { get; set; }
    }
}