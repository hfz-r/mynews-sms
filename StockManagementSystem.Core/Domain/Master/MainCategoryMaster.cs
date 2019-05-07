using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Master
{
    public class MainCategoryMaster : BaseEntity
    {
        public int P_Code { get; set; }

        public string P_Name { get; set; }

        public byte Status { get; set; }    
    }
}
