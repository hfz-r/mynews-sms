using System;
using StockManagementSystem.Core.Domain.Settings;
using System.Collections.Generic;

namespace StockManagementSystem.Core.Domain.Items
{
    public class Item : BaseEntity, IAppendTimestamps
    {
        public string P_StockCode { get; set; }

        public string P_Desc { get; set; }

        public int? P_GroupId { get; set; }

        public double? P_SPrice1 { get; set; }

        public double? P_SPrice2 { get; set; }

        public double? P_SPrice3 { get; set; }

        public double? P_SPrice4 { get; set; }

        public double? P_SPrice5 { get; set; }

        public double? P_SPrice6 { get; set; }

        public double? P_SPrice7 { get; set; }

        public double? P_SPrice8 { get; set; }

        public double? P_SPrice9 { get; set; }

        public double? P_SPrice10 { get; set; }

        public double? P_SPrice11 { get; set; }

        public double? P_SPrice12 { get; set; }

        public double? P_SPrice13 { get; set; }

        public double? P_SPrice14 { get; set; }

        public double? P_SPrice15 { get; set; }

        public string P_RecStatus { get; set; }

        public int P_OrderStatus { get; set; }

        public int P_StockType { get; set; }

        public string P_Variant1 { get; set; }

        public string P_Variant2 { get; set; }

        public int? VendorId { get; set; }

        public virtual ICollection<ShelfLocation> ShelfLocations { get; set; }

        #region IAppendTimestamps members

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? ModifiedOnUtc { get; set; }

        #endregion
    }
}
