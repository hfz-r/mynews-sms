using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using StockManagementSystem.Api.Domain;
using StockManagementSystem.Api.DTOs.Devices;
using StockManagementSystem.Api.DTOs.Generics;
using StockManagementSystem.Api.DTOs.Items;
using StockManagementSystem.Api.DTOs.Master;
using StockManagementSystem.Api.DTOs.OrderLimit;
using StockManagementSystem.Api.DTOs.PushNotifications;
using StockManagementSystem.Api.DTOs.Roles;
using StockManagementSystem.Api.DTOs.Stores;
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

            //device
            CreateMap<Device, DeviceDto>();

            //item
            CreateMap<Item, ItemDto>();

            //user password
            CreateMap<UserPassword, UserPasswordDto>();

            //generic class
            CreateMap<Transaction, TransactionDto>();
            CreateMap<ShelfLocation, ShelfLocationDto>();
            CreateMap<TransporterTransaction, TransporterTransactionDto>();
            CreateMap<ASNDetailMaster, ASNDetailMasterDto>();
            CreateMap<ASNHeaderMaster, ASNHeaderMasterDto>();
            CreateMap<BarcodeMaster, BarcodeMasterDto>();
            CreateMap<BranchMaster, BranchMasterDto>();
            CreateMap<MainCategoryMaster, MainCategoryMasterDto>();
            CreateMap<OrderBranchMaster, OrderBranchMasterDto>();
            CreateMap<SalesMaster, SalesMasterDto>();
            CreateMap<ShelfLocationMaster, ShelfLocationMasterDto>();
            CreateMap<StockTakeControlMaster, StockTakeControlMasterDto>();
            CreateMap<StockTakeRightMaster, StockTakeRightMasterDto>();
            CreateMap<ShiftControlMaster, ShiftControlMasterDto>();
            CreateMap<SubCategoryMaster, SubCategoryMasterDto>();
            CreateMap<SupplierMaster, SupplierMasterDto>();

            CreateClientMaps();
            CreateUserMaps();
            CreateStoreMaps();
            CreateRoleMaps();
            CreateOrderLimitMaps();
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
                .ForMember(model => model.RecStatus, z => z.MapFrom(e => e.P_RecStatus))
                .ForMember(model => model.CompId, z => z.MapFrom(e => e.P_CompID))
                .ForMember(model => model.SellPriceLevel, z => z.MapFrom(e => e.P_SellPriceLevel))
                .ForMember(model => model.AreaCode, z => z.MapFrom(e => e.P_AreaCode))
                .ForMember(model => model.Address1, z => z.MapFrom(e => e.P_Addr1))
                .ForMember(model => model.Address2, z => z.MapFrom(e => e.P_Addr2))
                .ForMember(model => model.Address3, z => z.MapFrom(e => e.P_Addr3))
                .ForMember(model => model.State, z => z.MapFrom(e => e.P_State))
                .ForMember(model => model.City, z => z.MapFrom(e => e.P_City))
                .ForMember(model => model.Country, z => z.MapFrom(e => e.P_Country))
                .ForMember(model => model.PostCode, z => z.MapFrom(e => e.P_PostCode))
                .ForMember(model => model.Brand, z => z.MapFrom(e => e.P_Brand));
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

        private static void CreateOrderLimitMaps()
        {
            AutoMapperApiConfiguration.MapperConfigurationExpression.CreateMap<OrderLimit, OrderLimitDto>()
                .IgnoreAllNonExisting()
                .ForMember(model => model.StoreIds,
                    y => y.MapFrom(entity => entity.OrderLimitStores.Select(x => x.StoreId)))
                .ForMember(model => model.Stores,
                    y => y.MapFrom(entity =>
                        entity.OrderLimitStores.Select(x => x.Store.GetWithDefault(w => w, new Store()).ToDto())));
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