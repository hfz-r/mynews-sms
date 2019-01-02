namespace StockManagementSystem.Services.Users
{
    public static partial class UserDefaults
    {
        #region Cache Keys

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : userId
        /// </remarks>
        public static string GetUserByIdKey => "user.getuserbyid-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string UsersPatternCacheKey => "user.";

        #endregion

        #region User attributes

       

        #endregion
    }
}