namespace StockManagementSystem.Core.Domain.Tenants
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public bool SslEnabled { get; set; }

        public string Hosts { get; set; }

        public int DisplayOrder { get; set; }
    }
}