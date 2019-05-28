using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using StockManagementSystem.Api.Domain;
using StockManagementSystem.Api.DTOs.Devices;
using StockManagementSystem.Api.DTOs.Items;
using StockManagementSystem.Api.DTOs.Master;
using StockManagementSystem.Api.DTOs.PushNotifications;
using StockManagementSystem.Api.DTOs.Roles;
using StockManagementSystem.Api.DTOs.ShelfLocation;
using StockManagementSystem.Api.DTOs.Stores;
using StockManagementSystem.Api.DTOs.Transactions;
using StockManagementSystem.Api.DTOs.TransporterTransaction;
using StockManagementSystem.Api.DTOs.Users;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Models.ApiSettings;
using StockManagementSystem.Api.Models.ApiSettings.Clients;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.Items;
using StockManagementSystem.Core.Domain.Master;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Transactions;
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

            //permission
            CreateMap<Permission, PermissionDto>();

            //item
            CreateMap<Item, ItemDto>();

            //user password
            CreateMap<UserPassword, UserPasswordDto>();

            //transaction
            CreateMap<Transaction, TransactionDto>();

            //transporter transaction
            CreateMap<TransporterTransaction, TransporterTransactionDto>();

            //shelf location
            CreateMap<ShelfLocation, ShelfLocationDto>();

            //master classes
            CreateMap<ASNDetailMaster, ASNDetailMasterDto>();
            CreateMap<ASNHeaderMaster, ASNHeaderMasterDto>();
            CreateMap<BarcodeMaster, BarcodeMasterDto>();
            CreateMap<MainCategoryMaster, MainCategoryMasterDto>();
            CreateMap<OrderBranchMaster, OrderBranchMasterDto>();
            CreateMap<SalesMaster, SalesMasterDto>();
            CreateMap<ShelfLocationMaster, ShelfLocationMasterDto>();
            CreateMap<StockTakeControlMaster, StockTakeControlMasterDto>();
            CreateMap<StockTakeRightMaster, StockTakeRightMasterDto>();
            CreateMap<StockTakeControlOutletMaster, StockTakeControlOutletMasterDto>();
            CreateMap<ShiftControlMaster, ShiftControlMasterDto>();
            CreateMap<StockSupplierMaster, StockSupplierMasterDto>();
            CreateMap<SubCategoryMaster, SubCategoryMasterDto>();
            CreateMap<SupplierMaster, SupplierMasterDto>();
            CreateMap<WarehouseDeliveryScheduleMaster, WarehouseDeliveryScheduleMasterDto>();

            CreateClientMaps();
            CreateRedirectUrisMaps();
            CreatePostLogoutUrisMaps();
            CreateCorsOriginsUrisMaps();
            CreateUserMaps();
            CreateStoreMaps();
            CreateDeviceMaps();
            CreateUserStoreMaps();
            CreateRoleMaps();
            CreateUserRoleMaps();
            CreatePermissionRolesMaps();
            CreatePushNotificationMaps();
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
                .ForMember(model => model.ClientSecret, x => x.MapFrom(entity => entity.ClientSecrets.FirstOrDefault().Description))
                .ForMember(model => model.AccessTokenLifetime, x => x.MapFrom(entity => entity.AccessTokenLifetime))
                .ForMember(model => model.RefreshTokenLifetime, x => x.MapFrom(entity => entity.AbsoluteRefreshTokenLifetime));
        }

        private static void CreateRedirectUrisMaps()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<ClientRedirectUri, RedirectUrisModel>()
                .IgnoreAllNonExisting()
                .ForMember(model => model.Url, x => x.MapFrom(entity => entity.RedirectUri));
        }

        private static void CreatePostLogoutUrisMaps()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression
                .CreateMap<ClientPostLogoutRedirectUri, PostLogoutUrisModel>()
                .IgnoreAllNonExisting()
                .ForMember(model => model.Url, x => x.MapFrom(entity => entity.PostLogoutRedirectUri));
        }

        private static void CreateCorsOriginsUrisMaps()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression
                .CreateMap<ClientCorsOrigin, CorsOriginUrisModel>()
                .IgnoreAllNonExisting()
                .ForMember(model => model.Url, x => x.MapFrom(entity => entity.Origin));
        }

        private static void CreateUserMaps()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<User, UserDto>()
                .IgnoreAllNonExisting()
                .ForMember(model => model.Id, y => y.MapFrom(entity => entity.Id))
                .ForMember(model => model.Roles,
                    y => y.MapFrom(entity =>
                        entity.Roles.GetWithDefault(x => x, new List<Role>()).Select(role => role.ToDto())))
                .ForMember(model => model.Stores,
                    y => y.MapFrom(entity =>
                        entity.AppliedStores.GetWithDefault(x => x, new List<Store>()).Select(store => store.ToDto())))
                .ForMember(model => model.RoleIds,
                    y => y.MapFrom(entity => entity.Roles.Select(e => e.Id)))
                .ForMember(model => model.StoreIds,
                    z => z.MapFrom(entity => entity.AppliedStores.Select(s => s.P_BranchNo)))
                .ForMember(model => model.UserPassword, options => options.Ignore());
        }

        private static void CreateStoreMaps()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Store, StoreDto>()
                .IgnoreAllNonExisting()
                .ForMember(model => model.BranchNo, z => z.MapFrom(e => e.P_BranchNo))
                .ForMember(model => model.Name, z => z.MapFrom(e => e.P_Name))
                .ForMember(model => model.Status, z => z.MapFrom(e => e.Status))
                .ForMember(model => model.AreaCode, z => z.MapFrom(e => e.P_AreaCode))
                .ForMember(model => model.Address1, z => z.MapFrom(e => e.P_Addr1))
                .ForMember(model => model.Address2, z => z.MapFrom(e => e.P_Addr2))
                .ForMember(model => model.Address3, z => z.MapFrom(e => e.P_Addr3))
                .ForMember(model => model.State, z => z.MapFrom(e => e.P_State))
                .ForMember(model => model.City, z => z.MapFrom(e => e.P_City))
                .ForMember(model => model.Country, z => z.MapFrom(e => e.P_Country))
                .ForMember(model => model.Postcode, z => z.MapFrom(e => e.P_Postcode))
                .ForMember(model => model.PriceLevel, z => z.MapFrom(e => e.P_PriceLevel))
                .ForMember(model => model.DBIPAddress, z => z.MapFrom(e => e.P_DBIPAddress))
                .ForMember(model => model.DBName, z => z.MapFrom(e => e.P_DBName))
                .ForMember(model => model.DBUsername, z => z.MapFrom(e => e.P_DBUsername))
                .ForMember(model => model.DBPassword, z => z.MapFrom(e => e.P_DBPassword));
        }

        private static void CreateDeviceMaps()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Device, DeviceDto>()
                .IgnoreAllNonExisting()
                .ForMember(model => model.StoreDto,
                    y => y.MapFrom(entity => entity.Store.GetWithDefault(store => store, new Store()).ToDto()));
        }

        private static void CreateUserStoreMaps()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<UserStore, UserStoreDto>()
                .IgnoreAllNonExisting()
                .ForMember(model => model.StoreDto,
                    y => y.MapFrom(entity => entity.Store.GetWithDefault(store => store, new Store()).ToDto()))
                .ForMember(model => model.UserDto,
                    y => y.MapFrom(entity => entity.User.GetWithDefault(user => user, new User()).ToDto()));
        }

        private static void CreateRoleMaps()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<Role, RoleDto>()
                .IgnoreAllNonExisting()
                .ForMember(model => model.PermissionIds,
                    y => y.MapFrom(entity => entity.PermissionRoles.Select(x => x.PermissionId)))
                .ForMember(model => model.Permissions,
                    y => y.MapFrom(entity =>
                        entity.PermissionRoles.Select(
                            x => x.Permission.GetWithDefault(w => w, new Permission()).ToDto())));
        }

        private static void CreateUserRoleMaps()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<UserRole, UserRoleDto>()
                .IgnoreAllNonExisting()
                .ForMember(model => model.UserDto,
                    y => y.MapFrom(entity => entity.User.GetWithDefault(user => user, new User()).ToDto()))
                .ForMember(model => model.RoleDto,
                    y => y.MapFrom(entity => entity.Role.GetWithDefault(role => role, new Role()).ToDto()));
        }

        private static void CreatePermissionRolesMaps()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<PermissionRoles, PermissionRolesDto>()
                .IgnoreAllNonExisting()
                .ForMember(model => model.RoleDto,
                    y => y.MapFrom(entity => entity.Role.GetWithDefault(role => role, new Role()).ToDto()))
                .ForMember(model => model.PermissionDto,
                    y => y.MapFrom(entity => entity.Permission.GetWithDefault(permission => permission, new Permission()).ToDto()));
        }

        private static void CreatePushNotificationMaps()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<PushNotification, PushNotificationDto>()
                .IgnoreAllNonExisting()
                .ForMember(model => model.StoreIds,
                    y => y.MapFrom(entity => entity.PushNotificationStores.Select(x => x.StoreId)))
                .ForMember(model => model.Stores,
                    y => y.MapFrom(entity =>
                        entity.PushNotificationStores.Select(x =>
                            x.Store.GetWithDefault(w => w, new Store()).ToDto())));
        }

        public int Order => 0;
    }
}