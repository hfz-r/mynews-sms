namespace StockManagementSystem.Core.Domain.Master
{
    public class StockTakeRightMaster : BaseEntity
    {
        public string P_Role { get; set; }

        public int P_StockTakeNo { get; set; }

        public byte Status { get; set; }
    }
}
