namespace StockManagementSystem.Core.Domain.Master
{
    public class ShiftControlMaster : BaseEntity
    {
        public string P_StockCode { get; set; }

        public byte Status { get; set; }
    }
}
