using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Security
{
    public class StandardPermissionProvider : IPermissionProvider
    {
        public static readonly Permission AccessPanel =
            new Permission { Name = "Access panel", SystemName = "AccessPanel", Category = "Standard" };

        public static readonly Permission ManageUsers =
            new Permission { Name = "Manage Users", SystemName = "ManageUsers", Category = "User Management" };

        public static readonly Permission ManageActivityLog =
            new Permission { Name = "Manage Activity Log", SystemName = "ManageActivityLog", Category = "Configuration" };

        public static readonly Permission ManageDevices =
            new Permission { Name = "Manage Devices", SystemName = "ManageDevices", Category = "Device Management" };

        public static readonly Permission ManageReports =
            new Permission { Name = "Manage Report", SystemName = "ManageReports", Category = "Device Management" };

        public static readonly Permission ManageOrderLimit =
            new Permission { Name = "Manage Stock Order Limits", SystemName = "ManageOrderLimit", Category = "Configuration" };

        public static readonly Permission ManagePushNotification = 
            new Permission { Name = "Manage Push Notification", SystemName = "ManagePushNotification", Category = "Push Notification" };

        public static readonly Permission ManageLocation = 
            new Permission { Name = "Manage Locations",  SystemName = "ManageLocation", Category = "Setting" };

        public static readonly Permission ManageFormatSetting = 
            new Permission { Name = "Manage Format Setting",  SystemName = "ManageFormatSetting", Category = "Setting"  };

        public static readonly Permission ManageOutletManagement =
            new Permission { Name = "Manage Outlet Management", SystemName = "ManageOutletManagement", Category = "Outlet Management" };
        
        public static readonly Permission ManageReplenishmentSetting = new Permission
        {
            Name = "Manage Replenishment Setting",
            SystemName = "ManageReplenishmentSetting",
            Category = "Setting"

        };
        public static readonly Permission ManageStoreGroup =
            new Permission { Name = "Manage Outlet Grouping", SystemName = "ManageStoreGroup", Category = "Outlet Management" };

        public static readonly Permission ManageUserStore =
            new Permission { Name = "Manage Assigning User to Outlet", SystemName = "ManageUserStore", Category = "Outlet Management" };

        public static readonly Permission ManageAcl = 
            new Permission { Name = "Manage ACL", SystemName = "ManageAcl", Category = "Configuration" };

        public static readonly Permission ManagePlugins =
            new Permission { Name = "Manage Plugins", SystemName = "ManagePlugins", Category = "Configuration" };

        public static readonly Permission ManageMaintenance = 
            new Permission { Name = "Manage Maintenance", SystemName = "ManageMaintenance", Category = "Configuration" };

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
                ManageActivityLog,
                ManageDevices,
                ManageReports,
                ManageOrderLimit,
                ManagePushNotification,
                ManageLocation,
                ManageFormatSetting,
                ManageOutletManagement,
                ManageReplenishmentSetting,
                ManageStoreGroup,
                ManageUserStore,
                ManageAcl,
                ManagePlugins,
                ManageMaintenance,
            };
        }

        /// <summary>
        /// Get default permissions
        /// </summary>
        public virtual IEnumerable<DefaultPermission> GetDefaultPermissions()
        {
            return new[]
            {
                //TODO: disable sysadmin for others
                new DefaultPermission
                {
                    RoleSystemName = UserDefaults.SysAdminRoleName,
                    Permissions = new[]
                    {
                        AccessPanel,
                        ManageUsers,
                        ManageActivityLog,
                        ManageDevices,
                        ManageReports,
                        ManageOrderLimit,
                        ManagePushNotification,
                        ManageLocation,
                        ManageFormatSetting,
                        ManageOutletManagement,
                        ManageReplenishmentSetting,
                        ManageStoreGroup,
                        ManageUserStore,
                        ManageAcl,
                        ManagePlugins,
                        ManageMaintenance,
                    }
                },
                new DefaultPermission
                {
                    RoleSystemName = UserDefaults.AdministratorsRoleName,
                    Permissions = new[]
                    {
                        AccessPanel,
                        ManageUsers,
                        ManageDevices,
                        ManageReports,
                        ManageOrderLimit,
                        ManagePushNotification,
                        ManageLocation,
                        ManageFormatSetting,
                        ManageOutletManagement,
                        ManageStoreGroup,
                        ManageAcl,
                    }
                },
                new DefaultPermission
                {
                    RoleSystemName = UserDefaults.RegisteredRoleName,
                    Permissions = new[]
                    {
                        AccessPanel,
                        ManageDevices,
                        ManageReports,
                        ManageOrderLimit,
                        ManagePushNotification,
                        ManageLocation,
                        ManageFormatSetting,
                        ManageOutletManagement,
                    }
                },
                new DefaultPermission
                {
                    RoleSystemName = UserDefaults.GuestsRoleName,
                },
            };
        }
    }
}