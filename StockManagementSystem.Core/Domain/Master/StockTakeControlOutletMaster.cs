using System;

namespace StockManagementSystem.Core.Domain.Master
{
    public class StockTakeControlOutletMaster : BaseEntity
    {
        public int P_StockTakeNo { get; set; }

        public int P_BranchNo { get; set; }

        public byte Status { get; set; }
    }
}
