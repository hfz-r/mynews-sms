using System;
using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class OrderLimit : BaseEntity, IAppendTimestamps
    {
        //public int Percentage { get; set; } //Remove Percentage criteria; Not required - 05032019

        public int DeliveryPerWeek { get; set; }

        public int Safety { get; set; }

        public int InventoryCycle { get; set; }

        public int OrderRatio { get; set; }

        public virtual ICollection<OrderLimitStore> OrderLimitStores { get; set; }

        #region IAppendTimestamps members

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}