using System;
using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Stores
{
    public class StoreUserAssign : BaseEntity, IAppendTimestamps
    {
        public int StoreId { get; set; }

        public virtual Store Store { get; set; }

        public virtual ICollection<StoreUserAssignStores> StoreUserAssignStore { get; set; }

        #region IAppendTimestamps members

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}
