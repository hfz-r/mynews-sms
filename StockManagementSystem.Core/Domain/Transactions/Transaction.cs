using System;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Core.Domain.Transactions
{
    public class Transaction : BaseEntity, IAppendTimestamps
    {
        public int ModuleId { get; set; }

        public string DeviceSerialNo { get; set; }

        //key map to Store
        public int P_BranchNo { get; set; }

        public int? P_StaffNo { get; set; }

        public string P_StockCode { get; set; }

        public int? P_Qty { get; set; }

        public int? P_UnitMeasurementCode { get; set; }

        public string P_Remark { get; set; }

        public string P_Resend { get; set; }

        public string P_IssRef { get; set; }

        public string P_Doc { get; set; }

        public string P_Ref { get; set; }

        public string P_SysMod { get; set; }

        public string P_RecType { get; set; }

        public int? PcsQty { get; set; }

        public int? CtnQty { get; set; }

        public int? OtrQty { get; set; }

        public bool? IsPost { get; set; }

        public string P_ShiftNo { get; set; }

        public string P_ReasonCode { get; set; }

        public int? P_ParentId { get; set; }

        public int? P_GroupId { get; set; }

        public double? P_UnitCost { get; set; }

        public string P_Loc { get; set; }

        public double? P_SellPrice { get; set; }

        public double? P_Cost { get; set; }

        public string P_Desc { get; set; }

        public int? P_Unit { get; set; }

        public string STLocation { get; set; }

        public string Barcode { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string POSDoc { get; set; }

        public int? ContainerId { get; set; }

        public string LogNo { get; set; }

        public virtual Store Store { get; set; }

        //TODO: other navigation?

        #region IAppendTimestamps members

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? ModifiedOnUtc { get; set; }

        #endregion

    }
}