using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Items;
using System;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class ShelfLocation : BaseEntity, IAppendTimestamps
    {
        public int StoreId { get; set; }

        public string Stock_Code { get; set; }
        
        public string Location { get; set; }

        //public virtual Store Stores { set; get; }

        //public virtual Item Items { get; set; }

        #region IAppendTimestamps members

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}
