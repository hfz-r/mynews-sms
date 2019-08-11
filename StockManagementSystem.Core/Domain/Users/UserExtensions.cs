using System;
using System.Linq;

namespace StockManagementSystem.Core.Domain.Users
{
    public static class UserExtensions
    {
        /// <summary>
        /// Gets a value indicating whether user is in a certain role
        /// </summary>
        public static bool IsInRole(this User user, string roleSystemName, bool onlyActiveRoles = true)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(roleSystemName))
                throw new ArgumentNullException(nameof(roleSystemName));

            var result = user.Roles.FirstOrDefault(r => (!onlyActiveRoles || r.Active) && r.SystemName == roleSystemName) != null;
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether the user is a built-in record for background tasks
        /// </summary>
        public static bool IsBackgroundTaskAccount(this User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!user.IsSystemAccount || string.IsNullOrEmpty(user.SystemName))
                return false;

            var result = user.SystemName.Equals(UserDefaults.BackgroundTaskUserName, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        public static bool IsApiUser(this User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!user.IsSystemAccount || string.IsNullOrEmpty(user.SystemName))
                return false;

            var result = user.SystemName.Equals(UserDefaults.ApiUserName, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether user is administrator
        /// </summary>
        public static bool IsAdmin(this User user, bool onlyActiveRoles = true)
        {
            return IsInRole(user, UserDefaults.AdministratorsRoleName, onlyActiveRoles);
        }

        /// <summary>
        /// Gets a value indicating whether user is registered
        /// </summary>
        public static bool IsRegistered(this User user, bool onlyActiveRoles = true)
        {
            return IsInRole(user, UserDefaults.RegisteredRoleName, onlyActiveRoles);
        }

        /// <summary>
        /// Gets a value indicating whether user is guest
        /// </summary>
        public static bool IsGuest(this User user, bool onlyActiveCustomerRoles = true)
        {
            return IsInRole(user, UserDefaults.GuestsRoleName, onlyActiveCustomerRoles);
        }

        /// <summary>
        /// Get role identifiers
        /// </summary>
        public static int[] GetRoleIds(this User user, bool showHidden = false)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var rolesIds = user.Roles
                .Where(r => showHidden || r.Active)
                .Select(r => r.Id)
                .ToArray();

            return rolesIds;
        }
    }
}