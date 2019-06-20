using System;

namespace StockManagementSystem.Core.Domain.Master
{
    public class OrderBranchMaster : BaseEntity , IAppendTimestamps
    {
        public int P_DeliveryPerWeek { get; set; }

        public int P_Safety { get; set; }

        public int P_InventoryCycle { get; set; }

        public float P_OrderRatio { get; set; }

        public int? P_MinDays { get; set; }

        public int? P_MaxDays { get; set; }

        public int? P_FaceQty { get; set; }

        public byte Status { get; set; }

        #region IAppendTimestamps members

        public virtual string CreatedBy { get; set; }

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual string ModifiedBy { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}
