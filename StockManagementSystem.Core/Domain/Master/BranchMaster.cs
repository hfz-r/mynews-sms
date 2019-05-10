namespace StockManagementSystem.Core.Domain.Master
{
    public class BranchMaster : BaseEntity
    {
        public int P_BranchNo { get; set; }

        public string P_Name { get; set; }

        public string P_AreaCode { get; set; }

        public string P_Addr1 { get; set; }

        public string P_Addr2 { get; set; }

        public string P_Addr3 { get; set; }

        public string P_State { get; set; }

        public string P_City { get; set; }

        public string P_Country { get; set; }

        public int P_Postcode { get; set; }

        public int P_PriceLevel { get; set; }

        public string P_DBIPAddress { get; set; }

        public string P_DBName { get; set; }

        public string P_DBUsername { get; set; }

        public string P_DBPassword { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public byte Status { get; set; }
    }
}
