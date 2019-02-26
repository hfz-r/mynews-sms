namespace StockManagementSystem.Services.Tenants
{
    public static class TenantDefaults
    {
        #region Tenant mappings

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static string TenantMappingByEntityIdNameCacheKey => "tenantmapping.entityid-name-{0}-{1}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string TenantMappingPatternCacheKey => "tenantmapping.";

        #endregion

        #region Tenant

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string TenantsAllCacheKey => "tenants.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : tenant ID
        /// </remarks>
        public static string TenantsByIdCacheKey => "tenants.id-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string TenantsPatternCacheKey => "tenants.";

        #endregion
    }
}