namespace StockManagementSystem.Core.Domain.Master
{
    public class BarcodeMaster : BaseEntity
    {
        public string P_Barcode { get; set; }

        public string P_StockCode { get; set; }

        public int P_UM_ID { get; set; }

        public double P_UM_Qty { get; set; }

        public int P_PriorityID { get; set; }

        public string P_IssueRef { get; set; }

        public byte Status { get; set; }
    }
}
