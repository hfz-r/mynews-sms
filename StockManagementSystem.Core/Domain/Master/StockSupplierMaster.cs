namespace StockManagementSystem.Core.Domain.Master
{
    public class StockSupplierMaster : BaseEntity
    {
        public string P_StockCode { get; set; }

        public int P_SupplierNo { get; set; }

        public byte Status { get; set; }
    }
}
