using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Core.Domain.Security;

namespace StockManagementSystem.Services.Security
{
    public class StandardPermissionProvider : IPermissionProvider
    {
        public static readonly Permission AccessPanel = new Permission
        {
            Name = "Access panel",
            SystemName = "AccessPanel",
            Category = "Standard"
        };

        public static readonly Permission ManageUsers = new Permission
        {
            Name = "Manage Users",
            SystemName = "ManageUsers",
            Category = "User Management"
        };

        public static readonly Permission ManageRoles = new Permission
        {
            Name = "Manage Roles",
            SystemName = "ManageRoles",
            Category = "User Management"
        };

        public static readonly Permission ManagePermission = new Permission
        {
            Name = "Manage Permission",
            SystemName = "ManagePermission",
            Category = "User Management Configuration"
        };

        public static readonly Permission ManageActivityLog = new Permission
        {
            Name = "Manage Activity Log",
            SystemName = "ManageActivityLog",
            Category = "User Management Configuration"
        };

        //TODO: other modules

        /// <summary>
        /// Get permissions
        /// </summary>
        public virtual IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                AccessPanel,
                ManageUsers,
                ManageRoles,
                ManagePermission,
                ManageActivityLog,
            };
        }

        /// <summary>
        /// Get default permissions
        /// </summary>
        public virtual IEnumerable<DefaultPermission> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermission
                {
                    RoleSystemName = IdentityDefaults.AdministratorsRoleName,
                    Permissions = new[]
                    {
                        AccessPanel,
                        ManageUsers,
                        ManageRoles,
                        ManagePermission,
                        ManageActivityLog,
                    }
                },
                new DefaultPermission
                {
                    RoleSystemName = IdentityDefaults.ManagerRoleName,
                    Permissions = new[]
                    {
                        AccessPanel,
                        ManagePermission,
                    }
                },
                new DefaultPermission
                {
                    RoleSystemName = IdentityDefaults.RegisteredRoleName,
                    Permissions = new[]
                    {
                        AccessPanel,
                    }
                }
            };
        }
    }
}