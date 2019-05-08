using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class OrderLimitStore : BaseEntity
    {
        public int OrderLimitId { get; set; }

        public int StoreId { get; set; }

        public virtual OrderLimit OrderLimit { get; set; }

        public virtual Store Store { get; set; }
    }
}
