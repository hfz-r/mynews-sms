using System;

namespace StockManagementSystem.Core.Domain.Approvals
{
    public class Approval : BaseEntity
    {
        public bool IsApprovalEnabled { get; set; }
    }
}
