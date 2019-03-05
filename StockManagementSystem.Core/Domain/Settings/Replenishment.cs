using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class Replenishment : Entity
    {
        public int BufferDays { get; set; }

        public int ReplenishmentQty { get; set; }

        public virtual ICollection<ReplenishmentStore> ReplenishmentStores { get; set; }
    }
}
