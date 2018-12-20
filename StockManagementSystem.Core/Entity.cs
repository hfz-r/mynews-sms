using System;

namespace StockManagementSystem.Core
{
    /// <summary>
    /// Provided entity with additional info
    /// </summary>
    public abstract class Entity : BaseEntity
    {
        public string CreatedBy { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTimeOffset? ModifiedOn { get; set; }
    }
}