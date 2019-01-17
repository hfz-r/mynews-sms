using System;
using System.Linq;

namespace StockManagementSystem.Core.Domain.Identity
{
    public static class IdentityExtensions
    {
        /// <summary>
        /// Gets a value indicating whether user is in a certain role
        /// </summary>
        public static bool IsInRole(this User user, string roleSystemName)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(roleSystemName))
                throw new ArgumentNullException(nameof(roleSystemName));

            var result = user.UserRoles.FirstOrDefault(u => u.Role.SystemName == roleSystemName) != null;
            return result;
        }

        public static bool IsAdministrators(this User user)
        {
            return IsInRole(user, IdentityDefaults.AdministratorsRoleName);
        }

        public static bool IsRegistered(this User user)
        {
            return IsInRole(user, IdentityDefaults.RegisteredRoleName);
        }
    }
}