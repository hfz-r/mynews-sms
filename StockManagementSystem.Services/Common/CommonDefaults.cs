namespace StockManagementSystem.Services.Common
{
    public static partial class CommonDefaults
    {
        #region Cache Keys

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : key group
        /// </remarks>
        public static string GenericAttributeCacheKey => "common.genericattribute.{0}-{1}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string GenericAttributePatternCacheKey => "common.genericattribute.";

        #endregion
    }
}