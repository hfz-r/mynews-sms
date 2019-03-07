using System;
using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class Replenishment : BaseEntity, IAppendTimestamps
    {
        public int BufferDays { get; set; }

        public int ReplenishmentQty { get; set; }

        public virtual ICollection<ReplenishmentStore> ReplenishmentStores { get; set; }

        public DateTime CreatedOnUtc { get; set; }
        public DateTime? ModifiedOnUtc { get; set; }
    }
}
