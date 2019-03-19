using System.Linq;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using StockManagementSystem.Api.Domain;
using StockManagementSystem.Api.DTOs.Devices;
using StockManagementSystem.Api.DTOs.Items;
using StockManagementSystem.Api.DTOs.Roles;
using StockManagementSystem.Api.DTOs.Users;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Models.ApiSettings;
using StockManagementSystem.Api.Models.ApiSettings.Clients;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.Items;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Infrastructure.Mapper;

namespace StockManagementSystem.Api.Infrastructure.Mapper
{
    public class ApiMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public ApiMapperConfiguration()
        {
            //api configuration
            CreateMap<ApiSettings, ApiConfigurationModel>();
            CreateMap<ApiConfigurationModel, ApiSettings>();

            //role
            CreateMap<Role, RoleDto>();
            CreateMap<RoleDto, Role>();

            //device
            CreateMap<Device, DeviceDto>();

            //item
            CreateMap<Item, ItemDto>();

            CreateClientMaps();
            CreateUserMaps();
        }

        private new static void CreateMap<TSource, TDestination>()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression
                .CreateMap<TSource, TDestination>()
                .IgnoreAllNonExisting();
        }

        private static void CreateClientMaps()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Client, ClientModel>()
                .ForMember(model => model.ClientSecret,
                    x => x.MapFrom(entity => entity.ClientSecrets.FirstOrDefault().Description))
                .ForMember(model => model.RedirectUrl,
                    x => x.MapFrom(entity => entity.RedirectUris.FirstOrDefault().RedirectUri))
                .ForMember(model => model.AccessTokenLifetime, x => x.MapFrom(entity => entity.AccessTokenLifetime))
                .ForMember(model => model.RefreshTokenLifetime,
                    x => x.MapFrom(entity => entity.AbsoluteRefreshTokenLifetime));
        }

        private static void CreateUserMaps()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<User, UserDto>()
                .IgnoreAllNonExisting()
                .ForMember(model => model.Id, y => y.MapFrom(entity => entity.Id))
                .ForMember(model => model.RoleIds, y => y.MapFrom(entity => entity.Roles.Select(e => e.Id)));
        }

        public int Order => 0;
    }
}