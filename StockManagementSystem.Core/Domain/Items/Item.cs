using StockManagementSystem.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Items
{
    public class Item : Entity
    {
        public string P_Desc { get; set; }

        //public virtual ICollection<ShelfLocation> ShelfLocations { get; set; }
    }
}
