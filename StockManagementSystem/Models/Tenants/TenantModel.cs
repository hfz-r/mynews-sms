using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Tenants
{
    /// <summary>
    /// Represents a tenant model
    /// </summary>
    public class TenantModel : BaseEntityModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public virtual bool SslEnabled { get; set; }

        public string Hosts { get; set; }

        public int DisplayOrder { get; set; }
    }
}