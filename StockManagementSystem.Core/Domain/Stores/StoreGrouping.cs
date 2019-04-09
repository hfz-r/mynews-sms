using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Stores
{
    public class StoreGrouping : Entity
    {
        public string GroupName { get; set; }

        //public virtual ICollection<StoreGroupingStores> StoreGroupingStore { get; set; }

        public virtual ICollection<Store> Store { get; set; }
    }
}

    

