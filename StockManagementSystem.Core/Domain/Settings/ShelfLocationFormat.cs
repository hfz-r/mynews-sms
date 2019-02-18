using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class ShelfLocationFormat : BaseEntity
    {
        public string Prefix { get; set; }

        public string Name { get; set; }

        public virtual ICollection<ShelfLocation> ShelfLocations { set; get; }
    }
}
