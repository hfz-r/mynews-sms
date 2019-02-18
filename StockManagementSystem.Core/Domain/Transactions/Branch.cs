namespace StockManagementSystem.Core.Domain.Transactions
{
    /// <summary>
    /// Represent fake branch entity 
    /// </summary>
    public class Branch : BaseEntity
    {
        public string Name { get; set; }

        public string Location { get; set; }
    }
}