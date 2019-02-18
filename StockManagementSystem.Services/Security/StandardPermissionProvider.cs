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

        public static readonly Permission ManageAcl = 
            new Permission { Name = "Manage ACL", SystemName = "ManageAcl", Category = "Configuration" };

        public static readonly Permission ManagePlugins =
            new Permission { Name = "Manage Plugins", SystemName = "ManagePlugins", Category = "Configuration" };

        public static readonly Permission ManageMaintenance = 
            new Permission { Name = "Manage Maintenance", SystemName = "ManageMaintenance", Category = "Configuration" };

        public static readonly Permission ManageOutletManagement = new Permission
        {
            Name = "Manage Outlet Management",
            SystemName = "ManageOutletManagement",
            Category = "Outlet Management"
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
                ManageActivityLog,
                ManageDevices,
                ManageReports,
                ManageOrderLimit,
                ManageAcl,
                ManagePlugins,
                ManageMaintenance,
                ManageFormatSetting,
                ManageOutletManagement
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
                    RoleSystemName = UserDefaults.AdministratorsRoleName,
                    Permissions = new[]
                    {
                        AccessPanel,
                        ManageUsers,
                        ManageDevices,
                        ManageReports,
                        ManageActivityLog,
                        ManageOrderLimit,
                        ManageAcl,
                        ManagePlugins,
                        ManageMaintenance,
                        ManageFormatSetting,
                        ManageOutletManagement
                    }
                },
                new DefaultPermission
                {
                    RoleSystemName = UserDefaults.ManagersRoleName,
                    Permissions = new[]
                    {
                        AccessPanel,
                        ManageUsers,
                        ManageDevices,
                        ManageReports,
                        ManageOrderLimit,
                    }
                },
                new DefaultPermission
                {
                    RoleSystemName = UserDefaults.RegisteredRoleName,
                    Permissions = new[]
                    {
                        AccessPanel,
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