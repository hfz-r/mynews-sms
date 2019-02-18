using AutoMapper;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Transactions;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Infrastructure.Mapper;
using StockManagementSystem.Models.Devices;
using StockManagementSystem.Models.Logging;
using StockManagementSystem.Models.OrderLimits;
using StockManagementSystem.Models.Reports;
using StockManagementSystem.Models.PushNotifications;
using StockManagementSystem.Models.Locations;
using StockManagementSystem.Models.Roles;
using StockManagementSystem.Models.Stores;
using StockManagementSystem.Models.Users;
using StockManagementSystem.Web.Models;
using StockManagementSystem.Models.Setting;

namespace StockManagementSystem.Infrastructure.Mapper
{
    public class DefaultMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public DefaultMapperConfiguration()
        {
            CreateRoleMaps();
            CreateUserMaps();
            CreateLoggingMaps();
            CreateStoresMaps();
            CreateDeviceMaps();
            CreateOrderLimitMaps();
            CreateFakerMaps();
            CreatePushNotificationMaps();
            CreateShelfLocationFormatMaps();
            CreateFormatSettingMaps();
            CreateDeviceTrackingMaps();

            ForAllMaps((mapConfiguration, map) =>
            {
                //exclude Form and CustomProperties from mapping BaseModel
                if (typeof(BaseModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(BaseModel.Form), options => options.Ignore());
                    map.ForMember(nameof(BaseModel.CustomProperties), options => options.Ignore());
                }

                //exclude ActiveStoreScopeConfiguration from mapping ISettingsModel
                if (typeof(ISettingsModel).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(ISettingsModel.ActiveStoreScopeConfiguration), options => options.Ignore());

                //exclude some properties from mapping Permission supported models
                if (typeof(IAclSupported).IsAssignableFrom(mapConfiguration.DestinationType))
                    map.ForMember(nameof(IAclSupported.SubjectToAcl), options => options.Ignore());
                if (typeof(IAclSupportedModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(IAclSupportedModel.AvailableRoles), options => options.Ignore());
                    map.ForMember(nameof(IAclSupportedModel.SelectedRoleIds), options => options.Ignore());
                }
            });
        }

        /// <summary>
        /// Create roles maps
        /// </summary>
        protected virtual void CreateRoleMaps()
        {
            CreateMap<Role, RoleModel>();
            CreateMap<RoleModel, Role>()
                .ForMember(entity => entity.PermissionRoles, options => options.Ignore());
        }

        /// <summary>
        /// Create users maps
        /// </summary>
        protected virtual void CreateUserMaps()
        {
            CreateMap<ActivityLog, UserActivityLogModel>()
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.ActivityLogTypeName, options => options.Ignore());

            CreateMap<User, UserModel>()
                .ForMember(model => model.Email, options => options.Ignore())
                .ForMember(model => model.FullName, options => options.Ignore())
                .ForMember(model => model.Phone, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.LastActivityDate, options => options.Ignore())
                .ForMember(model => model.UserRolesName, options => options.Ignore())
                .ForMember(model => model.UsernamesEnabled, options => options.Ignore())
                .ForMember(model => model.Password, options => options.Ignore())
                .ForMember(model => model.GenderEnabled, options => options.Ignore())
                .ForMember(model => model.Gender, options => options.Ignore())
                .ForMember(model => model.FirstName, options => options.Ignore())
                .ForMember(model => model.LastName, options => options.Ignore())
                .ForMember(model => model.DateOfBirthEnabled, options => options.Ignore())
                .ForMember(model => model.DateOfBirth, options => options.Ignore())
                .ForMember(model => model.PhoneEnabled, options => options.Ignore())
                .ForMember(model => model.RegisteredInStore, options => options.Ignore())
                .ForMember(model => model.TimeZoneId, options => options.Ignore())
                .ForMember(model => model.AllowUsersToSetTimeZone, options => options.Ignore())
                .ForMember(model => model.AvailableTimeZones, options => options.Ignore())
                .ForMember(model => model.SendEmail, options => options.Ignore())
                .ForMember(model => model.LastVisitedPage, options => options.Ignore())
                .ForMember(model => model.UserActivityLogSearchModel, options => options.Ignore());

            CreateMap<UserModel, User>()
                .ForMember(entity => entity.UserGuid, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.LastActivityDateUtc, options => options.Ignore())
                .ForMember(entity => entity.FailedLoginAttempts, options => options.Ignore())
                .ForMember(entity => entity.CannotLoginUntilDateUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.IsSystemAccount, options => options.Ignore())
                .ForMember(entity => entity.SystemName, options => options.Ignore())
                .ForMember(entity => entity.LastLoginDateUtc, options => options.Ignore())
                .ForMember(entity => entity.UserRoles, options => options.Ignore())
                .ForMember(entity => entity.RegisteredInStoreId, options => options.Ignore());
        }

        /// <summary>
        /// Create logging maps 
        /// </summary>
        protected virtual void CreateLoggingMaps()
        {
            CreateMap<ActivityLog, ActivityLogModel>()
                .ForMember(model => model.ActivityLogTypeName, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.UserEmail, options => options.Ignore());
            CreateMap<ActivityLogModel, ActivityLog>()
                .ForMember(entity => entity.ActivityLogType, options => options.Ignore())
                .ForMember(entity => entity.ActivityLogTypeId, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.User, options => options.Ignore())
                .ForMember(entity => entity.EntityId, options => options.Ignore());

            CreateMap<ActivityLogType, ActivityLogTypeModel>();
            CreateMap<ActivityLogTypeModel, ActivityLogType>()
                .ForMember(entity => entity.SystemKeyword, options => options.Ignore());

            CreateMap<User, SignedInLogModel>()
                .ForMember(model => model.UserId, options => options.Ignore())
                .ForMember(model => model.LastLoginDate, options => options.Ignore());
            CreateMap<SignedInLogModel, User>()
                .ForMember(entity => entity.UserGuid, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.LastActivityDateUtc, options => options.Ignore())
                .ForMember(entity => entity.FailedLoginAttempts, options => options.Ignore())
                .ForMember(entity => entity.CannotLoginUntilDateUtc, options => options.Ignore())
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.IsSystemAccount, options => options.Ignore())
                .ForMember(entity => entity.SystemName, options => options.Ignore())
                .ForMember(entity => entity.LastLoginDateUtc, options => options.Ignore())
                .ForMember(entity => entity.AdminComment, options => options.Ignore())
                .ForMember(entity => entity.Username, options => options.Ignore())
                .ForMember(entity => entity.UserRoles, options => options.Ignore())
                .ForMember(entity => entity.Active, options => options.Ignore())
                .ForMember(entity => entity.RegisteredInStoreId, options => options.Ignore());
        }

        /// <summary>
        /// Create stores maps 
        /// </summary>
        protected virtual void CreateStoresMaps()
        {
            CreateMap<Store, StoreModel>()
                .ForMember(model => model.Name, options => options.Ignore());
            CreateMap<StoreModel, Store>()
                .ForMember(entity => entity.P_BranchNo, options => options.Ignore())
                .ForMember(entity => entity.P_RecStatus, options => options.Ignore())
                .ForMember(entity => entity.P_CompID, options => options.Ignore())
                .ForMember(entity => entity.P_SellPriceLevel, options => options.Ignore())
                .ForMember(entity => entity.P_AreaCode, options => options.Ignore())
                .ForMember(entity => entity.P_Addr1, options => options.Ignore())
                .ForMember(entity => entity.P_Addr2, options => options.Ignore())
                .ForMember(entity => entity.P_Addr3, options => options.Ignore())
                .ForMember(entity => entity.P_State, options => options.Ignore())
                .ForMember(entity => entity.P_City, options => options.Ignore())
                .ForMember(entity => entity.P_Country, options => options.Ignore())
                .ForMember(entity => entity.P_PostCode, options => options.Ignore())
                .ForMember(entity => entity.P_Brand, options => options.Ignore())
                .ForMember(entity => entity.Device, options => options.Ignore())
                .ForMember(entity => entity.PushNotificationStores, options => options.Ignore())
                .ForMember(entity => entity.OrderLimitStores, options => options.Ignore())
                .ForMember(entity => entity.ShelfLocations, options => options.Ignore());
        }

        /// <summary>
        /// Create devices maps
        /// </summary>
        protected virtual void CreateDeviceMaps()
        {
            CreateMap<Device, DeviceModel>();
            CreateMap<DeviceModel, Device>()
                .ForMember(entity => entity.SerialNo, options => options.Ignore())
                .ForMember(entity => entity.StoreId, options => options.Ignore())
                .ForMember(entity => entity.Store, options => options.Ignore())
                .ForMember(entity => entity.ModelNo, options => options.Ignore())
                .ForMember(entity => entity.Id, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.CreatedBy, options => options.Ignore())
                .ForMember(entity => entity.ModifiedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.ModifiedBy, options => options.Ignore());
        }

        /// <summary>
        /// Create order limit maps
        /// </summary>
        protected virtual void CreateOrderLimitMaps()
        {
            CreateMap<OrderLimit, OrderLimitModel>()
                .ForMember(model => model.StoreName, options => options.Ignore());
            CreateMap<OrderLimitModel, OrderLimit>()
                .ForMember(entity => entity.Percentage, options => options.Ignore())
                .ForMember(entity => entity.OrderLimitStores, options => options.Ignore());
        }

        /// <summary>
        /// Represent faker mapping entity
        /// </summary>
        protected virtual void CreateFakerMaps()
        {
            CreateMap<Transaction, TransActivityModel>()
                .ForMember(model => model.Branch, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore());
            CreateMap<TransActivityModel, Transaction>()
                .ForMember(entity => entity.Branch, options => options.Ignore())
                .ForMember(entity => entity.BranchId, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore());
        }
        
        /// <summary>
        /// Create push notification maps
        /// </summary>
        protected virtual void CreatePushNotificationMaps()
        {
            CreateMap<PushNotification, PushNotificationModel>()
                .ForMember(model => model.StoreName, options => options.Ignore())
                .ForMember(model => model.CategoryName, options => options.Ignore());
            CreateMap<PushNotificationModel, PushNotification>()
                .ForMember(entity => entity.Title, options => options.Ignore())
                .ForMember(entity => entity.Desc, options => options.Ignore())
                .ForMember(entity => entity.PushNotificationStores, options => options.Ignore());
        }

        /// <summary>
        /// Create shelf location format maps
        /// </summary>
        protected virtual void CreateShelfLocationFormatMaps()
        {
            CreateMap<ShelfLocationFormat, LocationModel>();
            CreateMap<LocationModel, ShelfLocationFormat>()
                .ForMember(entity => entity.Prefix, options => options.Ignore())
                .ForMember(entity => entity.Name, options => options.Ignore())
                .ForMember(entity => entity.ShelfLocations, options => options.Ignore());
        }

        /// <summary>
        /// Create format setting maps
        /// </summary>
        protected virtual void CreateFormatSettingMaps()
        {
            CreateMap<FormatSetting, ShelfModel>();
            CreateMap<ShelfModel, FormatSetting>()
                .ForMember(entity => entity.Format, options => options.Ignore())
                .ForMember(entity => entity.Prefix, options => options.Ignore())
                .ForMember(entity => entity.Name, options => options.Ignore());
                //.ForMember(entity => entity.Length, options => options.Ignore());

            CreateMap<FormatSetting, BarcodeModel>();
            CreateMap<BarcodeModel, FormatSetting>()
                .ForMember(entity => entity.Prefix, options => options.Ignore());
            //.ForMember(entity => entity.Name, options => options.Ignore())
            //.ForMember(entity => entity.Format, options => options.Ignore())
            //.ForMember(entity => entity.Length, options => options.Ignore());
        }

        /// <summary>
        /// Create format setting maps
        /// </summary>
        protected virtual void CreateDeviceTrackingMaps()
        {
            CreateMap<Device, MapDeviceModel>();
            CreateMap<MapDeviceModel, Device>()
                .ForMember(entity => entity.SerialNo, options => options.Ignore())
                .ForMember(entity => entity.ModelNo, options => options.Ignore())
                .ForMember(entity => entity.StoreId, options => options.Ignore())
                .ForMember(entity => entity.Status, options => options.Ignore())
                .ForMember(entity => entity.Store, options => options.Ignore());          
        }

        public int Order => 0;
    }
}