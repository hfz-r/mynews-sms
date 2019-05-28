using System;
using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Transactions;

namespace StockManagementSystem.Api.Services
{
    public class TransporterTransactionApiService : ITransporterTransactionApiService
    {
        private readonly IRepository<TransporterTransaction> _transporterTransactionRepository;

        public TransporterTransactionApiService(IRepository<TransporterTransaction> transporterTransactionRepository)
        {
            _transporterTransactionRepository = transporterTransactionRepository;
        }

        public IList<TransporterTransaction> GetTransporterTransaction(
            IList<int> ids = null, int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null)
        {
            var query = GetTransporterTransactionQuery(createdAtMin, createdAtMax, ids);

            if (sinceId > 0)
                query = query.Where(device => device.Id > sinceId);

            return new ApiList<TransporterTransaction>(query, page - 1, limit);
        }

        public TransporterTransaction GetTransporterTransactionById(int id)
        {
            if (id <= 0)
                return null;

            var transporterTransaction = _transporterTransactionRepository.Table.FirstOrDefault(i => i.Id == id);

            return transporterTransaction;
        }

        public int GetTransporterTransactionCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null)
        {
            var query = GetTransporterTransactionQuery(createdAtMin, createdAtMax);

            return query.Count();
        }

        private IQueryable<TransporterTransaction> GetTransporterTransactionQuery(DateTime? createdAtMin = null,
            DateTime? createdAtMax = null, IList<int> ids = null)
        {
            var query = _transporterTransactionRepository.Table;

            if (ids != null && ids.Count > 0)
                query = query.Where(d => ids.Contains(d.Id));

            if (createdAtMin != null)
                query = query.Where(d => d.CreatedOnUtc > createdAtMin.Value);

            if (createdAtMax != null)
                query = query.Where(d => d.CreatedOnUtc < createdAtMax.Value);

            query = query.OrderBy(d => d.Id);

            return query;
        }
    }
}