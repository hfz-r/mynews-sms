using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Stores
{
    public class Store : BaseEntity
    {
        public string P_BranchNo { get; set; }

        public string P_Name { get; set; }

        public string P_RecStatus { get; set; }

        public string P_CompID { get; set; }

        public string P_SellPriceLevel { get; set; }

        public string P_AreaCode { get; set; }

        public string P_Addr1 { get; set; }

        public string P_Addr2 { get; set; }

        public string P_Addr3 { get; set; }

        public string P_State { get; set; }

        public string P_City { get; set; }

        public string P_Country { get; set; }

        public string P_PostCode { get; set; }

        public string P_Brand { get; set; }
    }
}
