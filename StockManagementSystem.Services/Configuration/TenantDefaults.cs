namespace StockManagementSystem.Services.Configuration
{
    public static class TenantDefaults
    {
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
    }
}