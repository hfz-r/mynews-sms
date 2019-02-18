namespace StockManagementSystem.Services.Stores
{
    public static class StoreDefaults
    {
        #region Stores

        public static string StoresAllCacheKey => "stores.all";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : store branchno
        /// </remarks>
        public static string StoresByIdCacheKey => "stores.branchno-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string StoresPatternCacheKey => "stores.";

        #endregion
    }
}