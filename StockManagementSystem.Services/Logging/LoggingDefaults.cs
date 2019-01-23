namespace StockManagementSystem.Services.Logging
{
    public static partial class LoggingDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string ActivityTypeAllCacheKey => "activitytype.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string ActivityTypePatternCacheKey => "activitytype.";
    }
}