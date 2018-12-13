using System;
using System.ComponentModel.DataAnnotations;

namespace StockManagementSystem.Core
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    public abstract class BaseEntity
    {
        [MaxLength(450)]
        public int Id { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public DateTimeOffset? ModifiedOn { get; set; }
    }
}