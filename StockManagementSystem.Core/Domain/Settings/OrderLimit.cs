using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class OrderLimit : Entity
    {
        public int Percentage { get; set; }

        public int DaysofStock { get; set; }

        public int DaysofSales { get; set; }

        public virtual ICollection<OrderLimitStore> OrderLimitStores { get; set; }
    }
}
