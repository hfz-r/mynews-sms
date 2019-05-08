using System;

namespace StockManagementSystem.Core.Domain.Master
{
    public class SalesMaster : BaseEntity
    {
        public string P_StockCode { get; set; }

        public int P_BranchNo { get; set; }

        public int P_SalesQty { get; set; }

        public int P_OpeningBalanceQty { get; set; }

        public int P_OrderQty { get; set; }

        public int P_ReceiveQty { get; set; }

        public int P_ReturnQty { get; set; }

        public DateTime P_TxnDate { get; set; }

        public byte Status { get; set; }
    }
}
