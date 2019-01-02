namespace StockManagementSystem.Services.Security
{
    public static partial class SecurityDefaults
    {
        #region Cache Keys

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : role ID
        /// {1} : permission system name
        /// </remarks>
        public static string PermissionsAllowedKey => "permission.allowed-{0}-{1}";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : role ID
        /// </remarks>
        public static string GetPermissionByRoleIdKey => "permission.getpermissionrecordsbyroleid-{0}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string PermissionsPatternCacheKey => "permission.";

        #endregion

        #region Access control list

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : entity ID
        /// {1} : entity name
        /// </remarks>
        public static string AclRecordByEntityIdNameCacheKey => "aclrecord.entityid-name-{0}-{1}";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string AclRecordPatternCacheKey => "aclrecord.";

        #endregion

    }
}