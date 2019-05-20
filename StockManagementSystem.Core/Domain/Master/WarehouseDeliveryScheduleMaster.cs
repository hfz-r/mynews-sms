using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Master
{
    public class WarehouseDeliveryScheduleMaster : BaseEntity
    {
        public int P_BranchNo { get; set; }

        public byte P_Day1 { get; set; }

        public byte P_Day2 { get; set; }

        public byte P_Day3 { get; set; }

        public byte P_Day4 { get; set; }

        public byte P_Day5 { get; set; }

        public byte P_Day6 { get; set; }

        public byte P_Day7 { get; set; }

        public byte Status { get; set; }
    }
}
