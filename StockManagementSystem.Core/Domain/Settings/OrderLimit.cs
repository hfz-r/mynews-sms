using System;
using System.Collections.Generic;
using System.Text;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class OrderLimit : BaseEntity
    {
        public double Percentage { get; set; }

        public virtual ICollection<OrderLimitStore> OrderLimitStores { get; set; }
    }
}
