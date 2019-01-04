using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class ShelfLocationFormat : BaseEntity
    {
        public string Prefix1 { get; set; }

        public string Prefix2 { get; set; }

        public string Prefix3 { get; set; }

        public string Prefix4 { get; set; }

        public string Name1 { get; set; }

        public string Name2 { get; set; }

        public string Name3 { get; set; }

        public string Name4 { get; set; }

        public virtual ICollection<ShelfLocation> ShelfLocations { set; get; }
    }
}
