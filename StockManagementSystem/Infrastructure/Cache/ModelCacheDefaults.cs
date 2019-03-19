namespace StockManagementSystem.Infrastructure.Cache
{
    public static partial class ModelCacheDefaults
    {
        /// <summary>
        /// Key for logo
        /// </summary>
        /// <remarks>
        /// {0} : current tenant ID
        /// {1} : is connection SSL secured (included in a picture URL)
        /// </remarks>
        public static string LogoPath => "pres.logo-{0}-{1}";
        public static string LogoPathPatternKey => "pres.logo";
    }
}