using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class ReplenishmentStore : Entity
    {
        public int ReplenishmentId { get; set; }

        public int StoreId { get; set; }

        public virtual Replenishment Replenishment { get; set; }

        public virtual Store Store { get; set; }
    }
}
