using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class ReplenishmentStore : BaseEntity
    {
        public int ReplenishmentId { get; set; }

        public int StoreId { get; set; }

        public virtual Replenishment Replenishment { get; set; }

        public virtual Store Store { get; set; }
    }
}
