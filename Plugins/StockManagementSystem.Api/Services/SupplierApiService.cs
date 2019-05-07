using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public class SupplierApiService : ISupplierApiService
    {
        private readonly IRepository<SupplierMaster> _supplierRepository;

        public SupplierApiService(IRepository<SupplierMaster> supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public IList<SupplierMaster> GetSuppliers(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetSuppliersQuery(sinceId);

            return new ApiList<SupplierMaster>(query, page - 1, limit);
        }

        public SupplierMaster GetSupplierById(int id)
        {
            if (id <= 0)
                return null;

            var supplier = _supplierRepository.Table.FirstOrDefault(i => i.Id == id);

            return supplier;
        }

        public int GetSuppliersCount(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetSuppliersQuery(sinceId).Count();
        }

        private IQueryable<SupplierMaster> GetSuppliersQuery(int sinceId = 0)
        {
            var query = _supplierRepository.Table;

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}