using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Master
{
    public class WarehouseDeliveryScheduleMaster : BaseEntity
    {
        public int P_BranchNo { get; set; }

        public byte Day1 { get; set; }

        public byte Day2 { get; set; }

        public byte Day3 { get; set; }

        public byte Day4 { get; set; }

        public byte Day5 { get; set; }

        public byte Day6 { get; set; }

        public byte Day7 { get; set; }

        public byte Status { get; set; }
    }
}
