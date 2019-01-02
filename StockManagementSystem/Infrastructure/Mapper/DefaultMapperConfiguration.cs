﻿using AutoMapper;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Infrastructure.Mapper;
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
            CreateMap<User, UserModel>()
                .ForMember(model => model.Password, options => options.Ignore())
                .ForMember(model => model.CreatedOn, options => options.Ignore())
                .ForMember(model => model.LastActivityDate, options => options.Ignore())
                .ForMember(model => model.UserRolesName, options => options.Ignore())
                .ForMember(model => model.SendEmail, options => options.Ignore());
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

        public int Order => 0;
    }
}