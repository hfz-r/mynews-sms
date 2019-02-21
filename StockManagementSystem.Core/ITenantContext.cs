using StockManagementSystem.Core.Domain.Tenants;

namespace StockManagementSystem.Core
{
    /// <summary>
    /// Tenant context
    /// </summary>
    public interface ITenantContext
    {
        /// <summary>
        /// Gets the current tenant
        /// </summary>
        Tenant CurrentTenant { get; }

        /// <summary>
        /// Gets active tenant scope configuration
        /// </summary>
        int ActiveTenantScopeConfiguration { get; }
    }
}