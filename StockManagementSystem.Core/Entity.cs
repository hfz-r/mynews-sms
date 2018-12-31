using System;

namespace StockManagementSystem.Core
{
    /// <summary>
    /// Provided entity with additional info
    /// </summary>
    public abstract class Entity : BaseEntity
    {
        public string CreatedBy { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime? ModifiedOnUtc { get; set; }
    }
}