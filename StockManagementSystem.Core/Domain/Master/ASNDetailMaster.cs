namespace StockManagementSystem.Core.Domain.Master
{
    public class ASNDetailMaster : BaseEntity
    {
        public string P_ASN_No { get; set; }

        public int P_BranchNo { get; set; }

        public string P_StockCode { get; set; }

        public string P_IssueRef { get; set; }

        public int P_TotalQty { get; set; }

        public string P_ContainerID { get; set; }

        public byte Status { get; set; }
    }
}
