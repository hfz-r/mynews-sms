using System;

namespace StockManagementSystem.Core.Domain.Stores
{
    public class StoreGroupingStores : BaseEntity, IAppendTimestamps
    {
        public int StoreId { get; set; }

        public int StoreGroupingId { get; set; }

        public virtual StoreGrouping StoreGroupings { get; set; }

        public virtual Store Store { get; set; }

        #region IAppendTimestamps members

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}
