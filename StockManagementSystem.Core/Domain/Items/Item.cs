using System;

namespace StockManagementSystem.Core.Domain.Items
{
    public class Item : BaseEntity
    {
        public string P_StockCode { get; set; }

        public string P_Desc { get; set; }

        public int P_SubCategoryID { get; set; }

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

        public DateTime? P_ModifyDT { get; set; }

        public int P_OrderStatus { get; set; }

        public int P_DisplayShelfLife { get; set; }

        public byte Status { get; set; }
    }
}
