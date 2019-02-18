namespace StockManagementSystem.Services.Configuration
{
    public class ConfigurationDefaults
    {
        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string SettingsAllCacheKey => "setting.all";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string SettingsPatternCacheKey => "setting.";
    }
}