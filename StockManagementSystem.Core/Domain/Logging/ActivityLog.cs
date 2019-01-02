using System;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Core.Domain.Logging
{
    public class ActivityLog : BaseEntity
    {
        public int ActivityLogTypeId { get; set; }

        public int? EntityId { get; set; }

        public string EntityName { get; set; }

        public int UserId { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public virtual ActivityLogType ActivityLogType { get; set; }

        public virtual User User { get; set; }

        public virtual string IpAddress { get; set; }
    }
}