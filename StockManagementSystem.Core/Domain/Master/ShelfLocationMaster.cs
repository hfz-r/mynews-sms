namespace StockManagementSystem.Core.Domain.Master
{
    public class ShelfLocationMaster : BaseEntity
    {
        public string P_StockCode { get; set; }

        public int P_BranchNo { get; set; }

        public string P_Gondola { get; set; }

        public string P_ShelfRow { get; set; }

        public int P_HorizontalFacing { get; set; }

        public int P_VerticalFacing { get; set; }

        public int P_DepthFacing { get; set; }

        public int P_TotalDisplay { get; set; }

        public byte Status { get; set; }
    }
}
