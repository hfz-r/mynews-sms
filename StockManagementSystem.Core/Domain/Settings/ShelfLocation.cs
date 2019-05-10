using System;

namespace StockManagementSystem.Core.Domain.Settings
{
    public class ShelfLocation : BaseEntity, IAppendTimestamps
    {
        public int StoreId { get; set; }

        public string Location { get; set; }

        public string P_StockCode { get; set; }

        public int? P_BranchNo { get; set; }

        public string P_Gondola { get; set; }

        public string P_Row { get; set; }

        public string P_Face { get; set; }

        public bool? IsPost { get; set; }

        public string IssueRef { get; set; }

        #region IAppendTimestamps members

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}
