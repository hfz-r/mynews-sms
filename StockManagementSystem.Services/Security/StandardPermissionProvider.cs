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
            Category = "User Management"
        };

        public static readonly Permission ManageDevices = new Permission
        {
            Name = "Manage Devices",
            SystemName = "ManageDevices",
            Category = "Device Management"
        };

        public static readonly Permission ManageOrderLimit = new Permission
        {
            Name = "Manage Stock Order Limits",
            SystemName = "ManageOrderLimit",
            Category = "Setting"
        };

        public static readonly Permission ManagePushNotification = new Permission
        {
            Name = "Manage Push Notification",
            SystemName = "ManagePushNotification",
            Category = "Push Notification"
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
                ManageDevices,
                ManageOrderLimit,
                ManagePushNotification
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
                        ManageDevices,
                        ManageOrderLimit,
                        ManagePushNotification
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