namespace StockManagementSystem.Core.Domain.Users
{
    public static class UserDefaults
    {
        #region System roles

        public static string SysAdminRoleName => "SysAdmin";

        public static string AdministratorsRoleName => "Administrators";

        public static string RegisteredRoleName => "Registered";

        public static string GuestsRoleName => "Guests";

        #endregion

        #region System users

        /// <summary>
        /// Gets a system name of 'background task' user object
        /// </summary>
        public static string BackgroundTaskUserName => "BackgroundTask";

        #endregion

        #region User attributes

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'FirstName'
        /// </summary>
        public static string FirstNameAttribute => "FirstName";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LastName'
        /// </summary>
        public static string LastNameAttribute => "LastName";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'Gender'
        /// </summary>
        public static string GenderAttribute => "Gender";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'DateOfBirth'
        /// </summary>
        public static string DateOfBirthAttribute => "DateOfBirth";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'Phone'
        /// </summary>
        public static string PhoneAttribute => "Phone";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'TimeZoneId'
        /// </summary>
        public static string TimeZoneIdAttribute => "TimeZoneId";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'PasswordRecoveryToken'
        /// </summary>
        public static string PasswordRecoveryTokenAttribute => "PasswordRecoveryToken";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'PasswordRecoveryTokenDateGenerated'
        /// </summary>
        public static string PasswordRecoveryTokenDateGeneratedAttribute => "PasswordRecoveryTokenDateGenerated";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'LastVisitedPage'
        /// </summary>
        public static string LastVisitedPageAttribute => "LastVisitedPage";

        /// <summary>
        /// Gets a name of generic attribute to store the value of 'TenantScopeConfiguration'
        /// </summary>
        public static string TenantScopeConfigurationAttribute => "TenantScopeConfiguration";

        #endregion
    }
}