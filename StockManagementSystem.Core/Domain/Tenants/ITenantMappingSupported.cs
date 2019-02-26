namespace StockManagementSystem.Core.Domain.Tenants
{
    /// <summary>
    /// Represents an entity which supports tenant mapping
    /// </summary>
    public interface ITenantMappingSupported
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entity is limited/restricted to certain tenants
        /// </summary>
        bool LimitedToTenants { get; set; }
    }
}