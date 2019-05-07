namespace StockManagementSystem.Core.Domain.Master
{
    public class SupplierMaster : BaseEntity
    {
        public int P_SupplierNo { get; set; }

        public string P_Name { get; set; }

        public string P_Addr1 { get; set; }

        public string P_Addr2 { get; set; }

        public string P_Addr3 { get; set; }

        public string P_State { get; set; }

        public string P_City { get; set; }

        public string P_Country { get; set; }

        public int P_Postcode { get; set; }

        public char P_IsWarehouse { get; set; }

        public byte Status { get; set; }
    }
}
