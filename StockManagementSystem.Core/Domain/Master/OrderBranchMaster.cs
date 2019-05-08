using System;

namespace StockManagementSystem.Core.Domain.Master
{
    public class OrderBranchMaster : BaseEntity , IAppendTimestamps
    {
        public int P_BranchNo { get; set; }

        public int P_DeliveryPerWeek { get; set; }

        public int P_Safety { get; set; }

        public int P_InventoryCycle { get; set; }

        public int P_OrderRatio { get; set; }

        public byte Status { get; set; }

        #region IAppendTimestamps members

        public virtual DateTime CreatedBy { get; set; }

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedBy { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}
