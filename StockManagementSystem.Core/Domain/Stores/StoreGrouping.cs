using System;
using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Stores
{
    public class StoreGrouping : BaseEntity, IAppendTimestamps
    {
        public string GroupName { get; set; }

        public virtual ICollection<StoreGroupingStores> StoreGroupingStore { get; set; }

        #region IAppendTimestamps members

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}

    

