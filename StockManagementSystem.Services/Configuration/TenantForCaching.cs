using System;
using System.ComponentModel.DataAnnotations.Schema;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Domain.Configuration;

namespace StockManagementSystem.Services.Configuration
{
    /// <summary>
    /// Tenant (for caching)
    /// </summary>
    [Serializable]
    [NotMapped]
    public class TenantForCaching : Tenant, IEntityForCaching
    {
        public TenantForCaching()
        {
        }

        public TenantForCaching(Tenant t)
        {
            Id = t.Id;
            Name = t.Name;
            Url = t.Url;
            SslEnabled = t.SslEnabled;
            Hosts = t.Hosts;
        }
    }
}