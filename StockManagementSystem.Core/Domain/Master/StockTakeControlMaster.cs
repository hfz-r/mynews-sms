using System;

namespace StockManagementSystem.Core.Domain.Master
{
    public class StockTakeControlMaster : BaseEntity
    {
        public int P_StockTakeNo { get; set; }

        public int P_BranchNo { get; set; } //BranchParticipant 0: All outlet; 1: Selected outlet

        public DateTime P_BeginDate { get; set; }

        public DateTime P_EndDate { get; set; }

        public byte Status { get; set; }
    }
}
