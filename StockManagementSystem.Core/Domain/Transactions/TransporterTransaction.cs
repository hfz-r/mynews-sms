using System;

namespace StockManagementSystem.Core.Domain.Transactions
{
    public class TransporterTransaction : BaseEntity, IAppendTimestamps
    {
        public string DriverName { get; set; }

        public string VehiclePlateNo { get; set; }

        public string DocNo { get; set; }

        public string ContainerId { get; set; }

        public string ModuleCode { get; set; }

        public int? Qty { get; set; }

        public int BranchNo { get; set; }

        public string P_SysMod { get; set; }

        public int? P_StaffNo { get; set; }

        #region IAppendTimestamps members

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}