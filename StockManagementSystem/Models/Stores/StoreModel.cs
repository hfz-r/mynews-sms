using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Stores
{
    public class StoreModel : BaseEntityModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public virtual bool SslEnabled { get; set; }

        public string Hosts { get; set; }
    }
}