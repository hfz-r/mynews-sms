using System.Collections.Generic;
using StockManagementSystem.Models.Tenants;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Setting
{
    /// <summary>
    /// Represents a tenant scope configuration model
    /// </summary>
    public class TenantScopeConfigurationModel : BaseModel
    {
        public TenantScopeConfigurationModel()
        {
            Tenants = new List<TenantModel>();
        }

        public int TenantId { get; set; }

        public IList<TenantModel> Tenants { get; set; }
    }
}