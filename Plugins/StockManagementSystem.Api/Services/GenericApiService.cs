using System;
using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Api.DTOs;
using StockManagementSystem.Api.Extensions;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Core.Domain.Master;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Transactions;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Data;
using StockManagementSystem.Services.Logging;

namespace StockManagementSystem.Api.Services
{
    public class GenericApiService<T> : IGenericApiService<T> where T : BaseDto
    {
        private readonly ILogger _logger;

        public GenericApiService(ILogger logger)
        {
            _logger = logger;
        }

        #region Private methods

        private object GetInstance()
        {
            if (typeof(T).BaseType != typeof(BaseDto))
                throw new Exception("Generics only applied to 'BaseDto' subclass");

            var className = typeof(T).Name.Remove(typeof(T).Name.Length - 3);

            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .First(t => string.Equals(t.Name, className, StringComparison.Ordinal));

            return type != null ? Activator.CreateInstance(type) : null;
        }

        private static dynamic RepositoryActivator(Type type)
        {
            if (type == null)
                return null;

            var contextType = typeof(Repository<>).MakeGenericType(type);

            var context = Activator.CreateInstance(contextType, EngineContext.Current?.Resolve<IDbContext>());
            dynamic repository = EngineContext.Current?.ResolveUnregistered(context.GetType());

            var q = repository;

            return q;
        }

        #endregion

        public IList<T> GetEntities(
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false)
        {
            var instance = GetInstance();

            switch (instance)
            {
                case Store _:
                {
                    var repository = RepositoryActivator(typeof(Store));
                    var query = repository.Table as IQueryable<Store>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<Store>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case Permission _:
                {
                    var repository = RepositoryActivator(typeof(Permission));
                    var query = repository.Table as IQueryable<Permission>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<Permission>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case UserStore _:
                {
                    var repository = RepositoryActivator(typeof(UserStore));
                    var query = repository.Table as IQueryable<UserStore>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<UserStore>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case UserRole _:
                {
                    var repository = RepositoryActivator(typeof(UserRole));
                    var query = repository.Table as IQueryable<UserRole>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<UserRole>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case PermissionRoles _:
                {
                    var repository = RepositoryActivator(typeof(PermissionRoles));
                    var query = repository.Table as IQueryable<PermissionRoles>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<PermissionRoles>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case ASNDetailMaster _:
                {
                    var repository = RepositoryActivator(typeof(ASNDetailMaster));
                    var query = repository.Table as IQueryable<ASNDetailMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<ASNDetailMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case ASNHeaderMaster _:
                {
                    var repository = RepositoryActivator(typeof(ASNHeaderMaster));
                    var query = repository.Table as IQueryable<ASNHeaderMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<ASNHeaderMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case BarcodeMaster _:
                {
                    var repository = RepositoryActivator(typeof(BarcodeMaster));
                    var query = repository.Table as IQueryable<BarcodeMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<BarcodeMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case MainCategoryMaster _:
                {
                    var repository = RepositoryActivator(typeof(MainCategoryMaster));
                    var query = repository.Table as IQueryable<MainCategoryMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<MainCategoryMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case OrderBranchMaster _:
                {
                    var repository = RepositoryActivator(typeof(OrderBranchMaster));
                    var query = repository.Table as IQueryable<OrderBranchMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<OrderBranchMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case SalesMaster _:
                {
                    var repository = RepositoryActivator(typeof(SalesMaster));
                    var query = repository.Table as IQueryable<SalesMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<SalesMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case ShelfLocationMaster _:
                {
                    var repository = RepositoryActivator(typeof(ShelfLocationMaster));
                    var query = repository.Table as IQueryable<ShelfLocationMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<ShelfLocationMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case ShiftControlMaster _:
                {
                    var repository = RepositoryActivator(typeof(ShiftControlMaster));
                    var query = repository.Table as IQueryable<ShiftControlMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<ShiftControlMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case StockTakeControlMaster _:
                {
                    var repository = RepositoryActivator(typeof(StockTakeControlMaster));
                    var query = repository.Table as IQueryable<StockTakeControlMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<StockTakeControlMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case StockTakeRightMaster _:
                {
                    var repository = RepositoryActivator(typeof(StockTakeRightMaster));
                    var query = repository.Table as IQueryable<StockTakeRightMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<StockTakeRightMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case StockTakeControlOutletMaster _:
                {
                    var repository = RepositoryActivator(typeof(StockTakeControlOutletMaster));
                    var query = repository.Table as IQueryable<StockTakeControlOutletMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<StockTakeControlOutletMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case StockSupplierMaster _:
                {
                    var repository = RepositoryActivator(typeof(StockSupplierMaster));
                    var query = repository.Table as IQueryable<StockSupplierMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<StockSupplierMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case SubCategoryMaster _:
                {
                    var repository = RepositoryActivator(typeof(SubCategoryMaster));
                    var query = repository.Table as IQueryable<SubCategoryMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<SubCategoryMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case SupplierMaster _:
                {
                    var repository = RepositoryActivator(typeof(SupplierMaster));
                    var query = repository.Table as IQueryable<SupplierMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<SupplierMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                case WarehouseDeliveryScheduleMaster _:
                {
                    var repository = RepositoryActivator(typeof(WarehouseDeliveryScheduleMaster));
                    var query = repository.Table as IQueryable<WarehouseDeliveryScheduleMaster>;

                    query = query.GetQueryDynamic(sortColumn, @descending, sinceId);

                    return new ApiList<WarehouseDeliveryScheduleMaster>(query, page - 1, limit).Select(entity => entity.ToDto())
                        .ToList() as IList<T>;
                }
                default:
                {
                    _logger.Information("Null.");

                    return null;
                }
            }
            //master table - end
        }

        public T GetEntityById(int id)
        {
            if (id == 0)
                return null;

            var instance = GetInstance();

            switch (instance)
            {
                case Store _:
                {
                    var repository = RepositoryActivator(typeof(Store));
                    var entity = (repository.Table as IQueryable<Store>)?.FirstOrDefault(e => e.P_BranchNo == id);

                    return entity.ToDto() as T;
                }
                case Permission _:
                {
                    var repository = RepositoryActivator(typeof(Permission));
                    var entity = (repository.Table as IQueryable<Permission>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case ASNDetailMaster _:
                {
                    var repository = RepositoryActivator(typeof(ASNDetailMaster));
                    var entity = (repository.Table as IQueryable<ASNDetailMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case ASNHeaderMaster _:
                {
                    var repository = RepositoryActivator(typeof(ASNHeaderMaster));
                    var entity = (repository.Table as IQueryable<ASNHeaderMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case BarcodeMaster _:
                {
                    var repository = RepositoryActivator(typeof(BarcodeMaster));
                    var entity = (repository.Table as IQueryable<BarcodeMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case MainCategoryMaster _:
                {
                    var repository = RepositoryActivator(typeof(MainCategoryMaster));
                    var entity = (repository.Table as IQueryable<MainCategoryMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case OrderBranchMaster _:
                {
                    var repository = RepositoryActivator(typeof(OrderBranchMaster));
                    var entity = (repository.Table as IQueryable<OrderBranchMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case SalesMaster _:
                {
                    var repository = RepositoryActivator(typeof(SalesMaster));
                    var entity = (repository.Table as IQueryable<SalesMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case ShelfLocationMaster _:
                {
                    var repository = RepositoryActivator(typeof(ShelfLocationMaster));
                    var entity = (repository.Table as IQueryable<ShelfLocationMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case ShiftControlMaster _:
                {
                    var repository = RepositoryActivator(typeof(ShiftControlMaster));
                    var entity = (repository.Table as IQueryable<ShiftControlMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case StockTakeControlMaster _:
                {
                    var repository = RepositoryActivator(typeof(StockTakeControlMaster));
                    var entity =
                        (repository.Table as IQueryable<StockTakeControlMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case StockTakeRightMaster _:
                {
                    var repository = RepositoryActivator(typeof(StockTakeRightMaster));
                    var entity =
                        (repository.Table as IQueryable<StockTakeRightMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case StockTakeControlOutletMaster _:
                {
                    var repository = RepositoryActivator(typeof(StockTakeControlOutletMaster));
                    var entity =
                        (repository.Table as IQueryable<StockTakeControlOutletMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case StockSupplierMaster _:
                {
                    var repository = RepositoryActivator(typeof(StockSupplierMaster));
                    var entity =
                        (repository.Table as IQueryable<StockSupplierMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case SubCategoryMaster _:
                {
                    var repository = RepositoryActivator(typeof(SubCategoryMaster));
                    var entity = (repository.Table as IQueryable<SubCategoryMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case SupplierMaster _:
                {
                    var repository = RepositoryActivator(typeof(SupplierMaster));
                    var entity = (repository.Table as IQueryable<SupplierMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                case WarehouseDeliveryScheduleMaster _:
                {
                    var repository = RepositoryActivator(typeof(WarehouseDeliveryScheduleMaster));
                    var entity =
                        (repository.Table as IQueryable<WarehouseDeliveryScheduleMaster>)?.FirstOrDefault(e => e.Id == id);

                    return entity.ToDto() as T;
                }
                default:
                {
                    _logger.Information("Null.");

                    return null;
                }
            }
            //master table - end
        }

        public int GetEntityCount()
        {
            var instance = GetInstance();

            switch (instance)
            {
                case Store _:
                {
                    var repository = RepositoryActivator(typeof(Store));

                    return (repository.Table as IQueryable<Store> ?? throw new InvalidOperationException())
                        .Count();
                }
                case Permission _:
                {
                    var repository = RepositoryActivator(typeof(Permission));

                    return (repository.Table as IQueryable<Permission> ?? throw new InvalidOperationException())
                        .Count();
                }
                case UserStore _:
                {
                    var repository = RepositoryActivator(typeof(UserStore));

                    return (repository.Table as IQueryable<UserStore> ?? throw new InvalidOperationException())
                        .Count();
                }
                case UserRole _:
                {
                    var repository = RepositoryActivator(typeof(UserRole));

                    return (repository.Table as IQueryable<UserRole> ?? throw new InvalidOperationException())
                        .Count();
                }
                case PermissionRoles _:
                {
                    var repository = RepositoryActivator(typeof(PermissionRoles));

                    return (repository.Table as IQueryable<PermissionRoles> ?? throw new InvalidOperationException())
                        .Count();
                }
                case ASNDetailMaster _:
                {
                    var repository = RepositoryActivator(typeof(ASNDetailMaster));

                    return (repository.Table as IQueryable<ASNDetailMaster> ?? throw new InvalidOperationException())
                        .Count();
                }
                case ASNHeaderMaster _:
                {
                    var repository = RepositoryActivator(typeof(ASNHeaderMaster));

                    return (repository.Table as IQueryable<ASNHeaderMaster> ?? throw new InvalidOperationException())
                        .Count();
                }
                case BarcodeMaster _:
                {
                    var repository = RepositoryActivator(typeof(BarcodeMaster));

                    return (repository.Table as IQueryable<BarcodeMaster> ?? throw new InvalidOperationException())
                        .Count();
                }
                case MainCategoryMaster _:
                {
                    var repository = RepositoryActivator(typeof(MainCategoryMaster));

                    return (repository.Table as IQueryable<MainCategoryMaster> ?? throw new InvalidOperationException())
                        .Count();
                }
                case OrderBranchMaster _:
                {
                    var repository = RepositoryActivator(typeof(OrderBranchMaster));

                    return (repository.Table as IQueryable<OrderBranchMaster> ?? throw new InvalidOperationException())
                        .Count();
                }
                case SalesMaster _:
                {
                    var repository = RepositoryActivator(typeof(SalesMaster));

                    return (repository.Table as IQueryable<SalesMaster> ?? throw new InvalidOperationException())
                        .Count();
                }
                case ShelfLocationMaster _:
                {
                    var repository = RepositoryActivator(typeof(ShelfLocationMaster));

                    return (repository.Table as IQueryable<ShelfLocationMaster> ??
                            throw new InvalidOperationException())
                        .Count();
                }
                case ShiftControlMaster _:
                {
                    var repository = RepositoryActivator(typeof(ShiftControlMaster));

                    return (repository.Table as IQueryable<ShiftControlMaster> ?? throw new InvalidOperationException())
                        .Count();
                }
                case StockTakeControlMaster _:
                {
                    var repository = RepositoryActivator(typeof(StockTakeControlMaster));

                    return (repository.Table as IQueryable<StockTakeControlMaster> ??
                            throw new InvalidOperationException())
                        .Count();
                }
                case StockTakeRightMaster _:
                {
                    var repository = RepositoryActivator(typeof(StockTakeRightMaster));

                    return (repository.Table as IQueryable<StockTakeRightMaster> ??
                            throw new InvalidOperationException())
                        .Count();
                }
                case StockTakeControlOutletMaster _:
                {
                    var repository = RepositoryActivator(typeof(StockTakeControlOutletMaster));

                    return (repository.Table as IQueryable<StockTakeControlOutletMaster> ??
                            throw new InvalidOperationException())
                        .Count();
                }
                case StockSupplierMaster _:
                {
                    var repository = RepositoryActivator(typeof(StockSupplierMaster));

                    return (repository.Table as IQueryable<StockSupplierMaster> ??
                            throw new InvalidOperationException())
                        .Count();
                }
                case SubCategoryMaster _:
                {
                    var repository = RepositoryActivator(typeof(SubCategoryMaster));

                    return (repository.Table as IQueryable<SubCategoryMaster> ?? throw new InvalidOperationException())
                        .Count();
                }
                case SupplierMaster _:
                {
                    var repository = RepositoryActivator(typeof(SupplierMaster));

                    return (repository.Table as IQueryable<SupplierMaster> ?? throw new InvalidOperationException())
                        .Count();
                }
                case WarehouseDeliveryScheduleMaster _:
                {
                    var repository = RepositoryActivator(typeof(WarehouseDeliveryScheduleMaster));

                    return (repository.Table as IQueryable<WarehouseDeliveryScheduleMaster> ??
                            throw new InvalidOperationException())
                        .Count();
                }
                default:
                {
                    _logger.Information("Null.");

                    return 0;
                }
            }
            //master table - end
        }
    }
}