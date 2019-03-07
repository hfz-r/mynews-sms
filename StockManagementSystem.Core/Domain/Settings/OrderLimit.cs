using System;
using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class OrderLimit : BaseEntity, IAppendTimestamps
    {
        public int Percentage { get; set; }

        public int DaysofStock { get; set; }

        public int DaysofSales { get; set; }

        public virtual ICollection<OrderLimitStore> OrderLimitStores { get; set; }

        #region IAppendTimestamps members

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}
