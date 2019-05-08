namespace StockManagementSystem.Core.Domain.Master
{
    public class OrderBranchMaster : BaseEntity
    {
        public int P_BranchNo { get; set; }

        public int P_DeliveryPerWeek { get; set; }

        public int P_Safety { get; set; }

        public int P_InventoryCycle { get; set; }

        public int P_OrderRatio { get; set; }

        public byte Status { get; set; }
    }
}
