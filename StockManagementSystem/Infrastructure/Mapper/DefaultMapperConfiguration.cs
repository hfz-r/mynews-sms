using AutoMapper;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Infrastructure.Mapper;
using StockManagementSystem.Models.Logging;
using StockManagementSystem.Models.Roles;
using StockManagementSystem.Models.Users;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Infrastructure.Mapper
{
    public class DefaultMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public DefaultMapperConfiguration()
        {
            CreateRoleMaps();
            CreateUserMaps();
            CreateLoggingMaps();

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
                .ForMember(entity => entity.UserRoles, options => options.Ignore());
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
                .ForMember(entity => entity.ModifiedBy, options => options.Ignore());
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
                .ForMember(entity => entity.EntityId, options => options.Ignore())
                .ForMember(entity => entity.EntityName, options => options.Ignore());

            CreateMap<ActivityLogType, ActivityLogTypeModel>();
            CreateMap<ActivityLogTypeModel, ActivityLogType>()
                .ForMember(entity => entity.SystemKeyword, options => options.Ignore());
        }

        public int Order => 0;
    }
}