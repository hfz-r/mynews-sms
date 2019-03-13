using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class ShelfLocation : Entity
    {
        public int StoreId { get; set; }

        public string Stock_Code { get; set; }
        
        public string Location { get; set; }

        //public virtual Store Stores { set; get; }

        //public virtual Item Items { get; set; }
    }
}
