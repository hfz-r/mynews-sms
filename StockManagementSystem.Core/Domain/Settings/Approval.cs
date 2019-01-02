using System;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class Approval : BaseEntity
    {
        public bool IsApprovalEnabled { get; set; }
    }
}
