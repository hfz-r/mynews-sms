namespace StockManagementSystem.Core.Domain.Master
{
    public class SubCategoryMaster : BaseEntity
    {
        public int P_Code { get; set; }

        public string P_Name { get; set; }

        public int P_MainCategoryID { get; set; }

        public byte Status { get; set; }
    }
}
