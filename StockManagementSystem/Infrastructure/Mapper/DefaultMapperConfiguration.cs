using AutoMapper;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Infrastructure.Mapper;
using StockManagementSystem.Models.Devices;
using StockManagementSystem.Models.Logging;
using StockManagementSystem.Models.OrderLimits;
using StockManagementSystem.Models.PushNotifications;
using StockManagementSystem.Models.Locations;
using StockManagementSystem.Models.Roles;
using StockManagementSystem.Models.Users;
using StockManagementSystem.Web.Models;
using StockManagementSystem.Models.Setting;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Models.Management;
using StockManagementSystem.Models.Replenishments;

namespace StockManagementSystem.Infrastructure.Mapper
{
    public class DefaultMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public DefaultMapperConfiguration()
        {
            CreateRoleMaps();
            CreateUserMaps();
            CreateLoggingMaps();
            CreateDeviceMaps();
            CreateOrderLimitMaps();
            CreatePushNotificationMaps();
            CreateShelfLocationFormatMaps();
            CreateFormatSettingMaps();
            CreateReplenishmentSettingMaps();
            CreateDeviceTrackingMaps();
            CreateOutletManagementMaps();

            ForAllMaps((mapConfiguration, map) =>
            {
                //exclude Form and CustomProperties from mapping BaseModel
                if (typeof(BaseModel).IsAssignableFrom(mapConfiguration.DestinationType))
                {
                    map.ForMember(nameof(BaseModel.Form), options => options.Ignore());
                    map.ForMember(nameof(BaseModel.CustomProperties), options => options.Ignore());
                }

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
                .ForMember(entity => entity.ConcurrencyStamp, options => options.Ignore())
                .ForMember(entity => entity.NormalizedName, options => options.Ignore())
                .ForMember(entity => entity.SystemName, options => options.Ignore())
                .ForMember(entity => entity.Claims, options => options.Ignore())
                .ForMember(entity => entity.UserRoles, options => options.Ignore())
                .ForMember(entity => entity.PermissionRoles, options => options.Ignore())
                .ForMember(entity => entity.CreatedBy, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.ModifiedBy, options => options.Ignore())
                .ForMember(entity => entity.ModifiedOnUtc, options => options.Ignore());
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
                .ForMember(model => model.Password, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.LastActivityDate, options => options.Ignore())
                .ForMember(model => model.UserRolesName, options => options.Ignore())
                .ForMember(model => model.SendEmail, options => options.Ignore())
                .ForMember(model => model.UserActivityLogSearchModel, options => options.Ignore());

            CreateMap<UserModel, User>()
                .ForMember(entity => entity.UserGuid, options => options.Ignore())
                .ForMember(entity => entity.AccessFailedCount, options => options.Ignore())
                .ForMember(entity => entity.ConcurrencyStamp, options => options.Ignore())
                .ForMember(entity => entity.LockoutEnabled, options => options.Ignore())
                .ForMember(entity => entity.LockoutEnd, options => options.Ignore())
                .ForMember(entity => entity.NormalizedEmail, options => options.Ignore())
                .ForMember(entity => entity.NormalizedUserName, options => options.Ignore())
                .ForMember(entity => entity.PasswordHash, options => options.Ignore())
                .ForMember(entity => entity.SecurityStamp, options => options.Ignore())
                .ForMember(entity => entity.LastActivityDateUtc, options => options.Ignore())
                .ForMember(entity => entity.LastLoginDateUtc, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.CreatedBy, options => options.Ignore())
                .ForMember(entity => entity.ModifiedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.ModifiedBy, options => options.Ignore())
                .ForMember(entity => entity.Logins, options => options.Ignore())
                .ForMember(entity => entity.Tokens, options => options.Ignore())
                .ForMember(entity => entity.Claims, options => options.Ignore())
                .ForMember(entity => entity.UserRoles, options => options.Ignore());
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
                .ForMember(entity => entity.AccessFailedCount, options => options.Ignore())
                .ForMember(entity => entity.ConcurrencyStamp, options => options.Ignore())
                .ForMember(entity => entity.LockoutEnabled, options => options.Ignore())
                .ForMember(entity => entity.LockoutEnd, options => options.Ignore())
                .ForMember(entity => entity.NormalizedEmail, options => options.Ignore())
                .ForMember(entity => entity.NormalizedUserName, options => options.Ignore())
                .ForMember(entity => entity.PasswordHash, options => options.Ignore())
                .ForMember(entity => entity.SecurityStamp, options => options.Ignore())
                .ForMember(entity => entity.AdminComment, options => options.Ignore())
                .ForMember(entity => entity.UserName, options => options.Ignore())
                .ForMember(entity => entity.Name, options => options.Ignore())
                .ForMember(entity => entity.LastActivityDateUtc, options => options.Ignore())
                .ForMember(entity => entity.LastLoginDateUtc, options => options.Ignore())
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.CreatedBy, options => options.Ignore())
                .ForMember(entity => entity.ModifiedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.ModifiedBy, options => options.Ignore())
                .ForMember(entity => entity.Logins, options => options.Ignore())
                .ForMember(entity => entity.Tokens, options => options.Ignore())
                .ForMember(entity => entity.Claims, options => options.Ignore())
                .ForMember(entity => entity.UserRoles, options => options.Ignore());
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
                //.ForMember(entity => entity.Percentage, options => options.Ignore()) //Remove Percentage criteria; Not required - 05032019
                .ForMember(entity => entity.OrderLimitStores, options => options.Ignore());
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
                .ForMember(entity => entity.Name, options => options.Ignore());
                //.ForMember(entity => entity.ShelfLocations, options => options.Ignore());
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
        /// Create replenishment maps
        /// </summary>
        protected virtual void CreateReplenishmentSettingMaps()
        {
            CreateMap<Replenishment, ReplenishmentModel>()
                .ForMember(model => model.StoreName, options => options.Ignore());
            CreateMap<ReplenishmentModel, Replenishment>()
                .ForMember(entity => entity.BufferDays, options => options.Ignore())
                .ForMember(entity => entity.ReplenishmentQty, options => options.Ignore())
                .ForMember(entity => entity.ReplenishmentStores, options => options.Ignore());
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

        /// <summary>
        /// Create outlet management maps
        /// </summary>
        protected virtual void CreateOutletManagementMaps()
        {
            CreateMap<StoreUserAssign, AssignUserModel>()
                .ForMember(model => model.LastActivityDate, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.User, options => options.Ignore())
                .ForMember(model => model.UserStoreName, options => options.Ignore())
                .ForMember(model => model.AvailableUsers, options => options.Ignore())
                .ForMember(model => model.AvailableUserStores, options => options.Ignore())
                .ForMember(model => model.SelectedUserIds, options => options.Ignore())
                .ForMember(model => model.SelectedUserStoreId, options => options.Ignore())
                .ForMember(model => model.StoreUsers, options => options.Ignore());

            CreateMap<AssignUserModel, StoreUserAssign>()
                .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.StoreId, options => options.Ignore())
                .ForMember(entity => entity.Store, options => options.Ignore())
                .ForMember(entity => entity.CreatedBy, options => options.Ignore())
                .ForMember(entity => entity.ModifiedBy, options => options.Ignore())
                .ForMember(entity => entity.ModifiedOnUtc, options => options.Ignore());

            CreateMap<StoreGrouping, GroupOutletModel>()
                .ForMember(model => model.StoreGroupings, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.LastActivityDate, options => options.Ignore())
                .ForMember(model => model.StoreName, options => options.Ignore())
                .ForMember(model => model.SelectedStoreIds, options => options.Ignore())
                .ForMember(model => model.AvailableStores, options => options.Ignore());
            CreateMap<GroupOutletModel, StoreGrouping>()
                 .ForMember(entity => entity.CreatedOnUtc, options => options.Ignore())
                .ForMember(entity => entity.StoreGroupingStore, options => options.Ignore())
                .ForMember(entity => entity.CreatedBy, options => options.Ignore())
                .ForMember(entity => entity.ModifiedBy, options => options.Ignore())
                .ForMember(entity => entity.ModifiedOnUtc, options => options.Ignore());
        }

        public int Order => 0;
    }
}