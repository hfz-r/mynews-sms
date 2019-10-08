using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Users;

namespace StockManagementSystem.Services.Security
{
    public class StandardPermissionProvider : IPermissionProvider
    {
        public static readonly Permission AccessPanel =
            new Permission {Name = "(Web) Access panel", SystemName = "AccessPanel", Category = "Standard"};

        public static readonly Permission ManageUsers =
            new Permission {Name = "(Web) Manage Users", SystemName = "ManageUsers", Category = "User Management"};

        public static readonly Permission ManageActivityLog =
            new Permission {Name = "(Web) Manage Activity Log", SystemName = "ManageActivityLog", Category = "Configuration"};

        public static readonly Permission ManageDevices =
            new Permission {Name = "(Web) Manage Devices", SystemName = "ManageDevices", Category = "Device Management"};

        public static readonly Permission ManageReports =
            new Permission {Name = "(Web) Manage Report", SystemName = "ManageReports", Category = "Device Management"};

        public static readonly Permission ManageOrderLimit =
            new Permission
            {
                Name = "(Web) Manage Stock Order Limits",
                SystemName = "ManageOrderLimit",
                Category = "Configuration"
            };

        public static readonly Permission ManagePushNotification =
            new Permission
            {
                Name = "(Web) Manage Push Notification",
                SystemName = "ManagePushNotification",
                Category = "Push Notification"
            };

        //public static readonly Permission ManageLocation =
        //    new Permission {Name = "Manage Locations", SystemName = "ManageLocation", Category = "Setting"};

        //public static readonly Permission ManageFormatSetting =
        //    new Permission {Name = "Manage Format Setting", SystemName = "ManageFormatSetting", Category = "Setting"};

        public static readonly Permission ManageOutletManagement =
            new Permission
            {
                Name = "(Web) Manage Outlet Management",
                SystemName = "ManageOutletManagement",
                Category = "Outlet Management"
            };

        //public static readonly Permission ManageReplenishmentSetting = new Permission
        //{
        //    Name = "Manage Replenishment Setting",
        //    SystemName = "ManageReplenishmentSetting",
        //    Category = "Setting"
        //};

        public static readonly Permission ManageStoreGroup =
            new Permission
            {
                Name = "(Web) Manage Outlet Grouping",
                SystemName = "ManageStoreGroup",
                Category = "Outlet Management"
            };

        public static readonly Permission ManageUserStore =
            new Permission
            {
                Name = "(Web) Manage Assigning User to Outlet",
                SystemName = "ManageUserStore",
                Category = "Outlet Management"
            };

        public static readonly Permission ManageAcl =
            new Permission { Name = "(Web) Manage ACL", SystemName = "ManageAcl", Category = "Configuration" };

        public static readonly Permission ManagePlugins =
            new Permission { Name = "(Web) Manage Plugins", SystemName = "ManagePlugins", Category = "Configuration" };

        public static readonly Permission ManageMaintenance =
            new Permission { Name = "(Web) Manage Maintenance", SystemName = "ManageMaintenance", Category = "Maintenance" };

        public static readonly Permission ManageSettings =
            new Permission { Name = "(Web) Manage Settings", SystemName = "ManageSettings", Category = "Configuration" };

        public static readonly Permission ManageSystemLog =
            new Permission { Name = "(Web) Manage System Log", SystemName = "ManageSystemLog", Category = "Maintenance" };

        #region HHT modules

        public static readonly Permission ManageTransporterReceive = new Permission
        {
            Name = "(HHT) Manage Transporter Receive",
            SystemName = "TRNS_RCV",
            Category = "Transporter Receive"
        };

        public static readonly Permission ManageStockReceive = new Permission
        {
            Name = "(HHT) Manage Stock Receive",
            SystemName = "RCV",
            Category = "Stock Receive"
        };

        public static readonly Permission ManageStockReceiveEnquiry = new Permission
        {
            Name = "(HHT) Manage Stock Receive Enquiry",
            SystemName = "RCV_ENQ",
            Category = "Stock Receive"
        };

        public static readonly Permission ManageFreshness = new Permission
        {
            Name = "(HHT) Manage Freshness",
            SystemName = "FRESH",
            Category = "Freshness"
        };

        public static readonly Permission ManageStockOrder = new Permission
        {
            Name = "(HHT) Manage Stock Order",
            SystemName = "ORD",
            Category = "Stock ORder"
        };

        public static readonly Permission ManageStockTakeResult = new Permission
        {
            Name = "(HHT) Manage Stock Take Result",
            SystemName = "ST_RES",
            Category = "Stock Take"
        };

        public static readonly Permission ManageStockTake = new Permission
        {
            Name = "(HHT) Manage Stock Take",
            SystemName = "ST",
            Category = "Stock Take"
        };

        public static readonly Permission ManageShiftStockTake = new Permission
        {
            Name = "(HHT) Manage Shift Stock Take",
            SystemName = "SHIFT_ST",
            Category = "Stock Take"
        };

        public static readonly Permission ManageZeroise = new Permission
        {
            Name = "(HHT) Manage Zeroise",
            SystemName = "ZERO",
            Category = "Stock Take"
        };

        public static readonly Permission ManageStockReturn = new Permission
        {
            Name = "(HHT) Manage Stock Return",
            SystemName = "RTN",
            Category = "Stock Return"
        };

        public static readonly Permission ManageStockReturnEnquiry = new Permission
        {
            Name = "(HHT) Manage Stock Return Enquiry",
            SystemName = "RTN_ENQ",
            Category = "Stock Return"
        };

        public static readonly Permission ManageTransporterCollection = new Permission
        {
            Name = "(HHT) Manage Transporter Collection",
            SystemName = "TRNS_COL",
            Category = "Transporter Collection"
        };

        public static readonly Permission ManageLocationRegistration = new Permission
        {
            Name = "(HHT) Manage Location Registration",
            SystemName = "LOC_REG",
            Category = "Location Control"
        };

        public static readonly Permission ManageLocationEnquiry = new Permission
        {
            Name = "(HHT) Manage Location Enquiry",
            SystemName = "LOC_ENQ",
            Category = "Location Control"
        };

        public static readonly Permission ManageTransferIn = new Permission
        {
            Name = "(HHT) Manage Transfer In",
            SystemName = "T_IN",
            Category = "Interbranch"
        };

        public static readonly Permission ManageTransferOut = new Permission
        {
            Name = "(HHT) Manage Transfer Out",
            SystemName = "T_OUT",
            Category = "Interbranch"
        };

        public static readonly Permission ManagePriceTagPrint = new Permission
        {
            Name = "(HHT) Manage Price Tag Print",
            SystemName = "PRN_PRC_TAG",
            Category = "Label Request"
        };

        public static readonly Permission ManageLocationBarcodePrint = new Permission
        {
            Name = "(HHT) Manage Location Barcode Print",
            SystemName = "PRN_LOC_BAR",
            Category = "Label Request"
        };

        public static readonly Permission ManagePlanogram = new Permission
        {
            Name = "(HHT) Manage Planogram",
            SystemName = "PGRAM",
            Category = "Planogram"
        };

        public static readonly Permission ManageNotificationSummary = new Permission
        {
            Name = "(HHT) Manage Notification Summary",
            SystemName = "NOT_SUM",
            Category = "Notification Summary"
        };

        public static readonly Permission ManageTransactionSummary = new Permission
        {
            Name = "(HHT) Manage Transaction Summary",
            SystemName = "TXN_SUM",
            Category = "Transaction Summary"
        };

        public static readonly Permission ManageSetting = new Permission
        {
            Name = "(HHT) Manage Settings",
            SystemName = "MNG_SET",
            Category = "Configuration"
        };

        #endregion

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
                //ManageLocation,
                //ManageFormatSetting,
                ManageOutletManagement,
                //ManageReplenishmentSetting,
                ManageStoreGroup,
                ManageUserStore,
                ManageAcl,
                ManagePlugins,
                ManageMaintenance,
                ManageSettings,
                ManageSystemLog,
                ManageTransporterReceive,
                ManageStockReceive,
                ManageStockReceiveEnquiry,
                ManageFreshness,
                ManageStockOrder,
                ManageStockTakeResult,
                ManageStockTake,
                ManageShiftStockTake,
                ManageZeroise,
                ManageStockReturn,
                ManageStockReturnEnquiry,
                ManageTransporterCollection,
                ManageLocationEnquiry,
                ManageLocationRegistration,
                ManageTransferIn,
                ManageTransferOut,
                ManagePriceTagPrint,
                ManageLocationBarcodePrint,
                ManagePlanogram,
                ManageNotificationSummary,
                ManageTransactionSummary,
                ManageSetting
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
                        //ManageLocation,
                        //ManageFormatSetting,
                        ManageOutletManagement,
                        //ManageReplenishmentSetting,
                        ManageStoreGroup,
                        ManageUserStore,
                        ManageAcl,
                        ManagePlugins,
                        ManageMaintenance,
                        ManageSettings,
                        ManageSystemLog,
                        ManageTransporterReceive,
                        ManageStockReceive,
                        ManageStockReceiveEnquiry,
                        ManageFreshness,
                        ManageStockOrder,
                        ManageStockTakeResult,
                        ManageStockTake,
                        ManageShiftStockTake,
                        ManageZeroise,
                        ManageStockReturn,
                        ManageStockReturnEnquiry,
                        ManageTransporterCollection,
                        ManageLocationEnquiry,
                        ManageLocationRegistration,
                        ManageTransferIn,
                        ManageTransferOut,
                        ManagePriceTagPrint,
                        ManageLocationBarcodePrint,
                        ManagePlanogram,
                        ManageNotificationSummary,
                        ManageTransactionSummary,
                        ManageSetting
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
                        //ManageLocation,
                        //ManageFormatSetting,
                        ManageOutletManagement,
                        //ManageReplenishmentSetting,
                        ManageStoreGroup,
                        ManageAcl,
                        ManageTransporterReceive,
                        ManageStockReceive,
                        ManageStockReceiveEnquiry,
                        ManageFreshness,
                        ManageStockOrder,
                        ManageStockTakeResult,
                        ManageStockTake,
                        ManageShiftStockTake,
                        ManageZeroise,
                        ManageStockReturn,
                        ManageStockReturnEnquiry,
                        ManageTransporterCollection,
                        ManageLocationEnquiry,
                        ManageLocationRegistration,
                        ManageTransferIn,
                        ManageTransferOut,
                        ManagePriceTagPrint,
                        ManageLocationBarcodePrint,
                        ManagePlanogram,
                        ManageNotificationSummary,
                        ManageTransactionSummary,
                        ManageSetting
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
                        //ManageLocation,
                        //ManageFormatSetting,
                        ManageOutletManagement,
                    }
                },
                new DefaultPermission
                {
                    RoleSystemName = UserDefaults.CashierRoleName,
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