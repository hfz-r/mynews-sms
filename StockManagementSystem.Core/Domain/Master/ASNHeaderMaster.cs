using System;

namespace StockManagementSystem.Core.Domain.Master
{
    public class ASNHeaderMaster : BaseEntity
    {
        public string P_ASN_No { get; set; }

        public DateTime P_DeliveryDate { get; set; }

        public int P_BranchNo { get; set; }

        public int P_SupplierNo { get; set; }

        public byte Status { get; set; }

        public DateTime? ImportDateTime { get; set; } //for Edi purpose only
    }
}
