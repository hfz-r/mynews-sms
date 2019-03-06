using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class OrderLimit : Entity
    {
        //public int Percentage { get; set; } //Remove Percentage criteria; Not required - 05032019

        public int DaysofStock { get; set; }

        public int DaysofSales { get; set; }

        public virtual ICollection<OrderLimitStore> OrderLimitStores { get; set; }
    }
}
