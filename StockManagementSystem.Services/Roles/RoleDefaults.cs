namespace StockManagementSystem.Services.Roles
{
    public static partial class RoleDefaults
    {
        #region Cache Keys

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        public static string GetRolesKey => "role.getroles";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : roleId
        /// </remarks>
        public static string GetRoleByIdKey => "role.getrolebyid-{0}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : systemName
        /// </remarks>
        public static string GetRoleBySystemName => "role.getrolebysystemname-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string RolesPatternCacheKey => "role.";

        #endregion
    }
}