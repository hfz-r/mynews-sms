using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class OrderLimit : Entity
    {
        public double Percentage { get; set; }

        public virtual ICollection<OrderLimitStore> OrderLimitStores { get; set; }
    }
}
