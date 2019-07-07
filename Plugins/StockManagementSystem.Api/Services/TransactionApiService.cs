using System;
using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Api.DTOs.Transactions;
using StockManagementSystem.Api.Extensions;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Transactions;
using static StockManagementSystem.Api.Extensions.ApiServiceExtension;

namespace StockManagementSystem.Api.Services
{
    public class TransactionApiService : ITransactionApiService
    {
        private readonly IRepository<Transaction> _transactionRepository;

        public TransactionApiService(IRepository<Transaction> transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public IList<Transaction> GetTransactions(
            IList<int> ids = null, int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null)
        {
            var query = GetTransactionQuery(createdAtMin, createdAtMax, ids);

            if (sinceId > 0)
                query = query.Where(device => device.Id > sinceId);

            return new ApiList<Transaction>(query, page - 1, limit);
        }

        public Transaction GetTransactionById(int id)
        {
            if (id <= 0)
                return null;

            var transaction = _transactionRepository.Table.FirstOrDefault(i => i.Id == id);

            return transaction;
        }

        public Search<TransactionDto> Search(
            string queryParams = "",
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false,
            bool count = false)
        {
            var query = _transactionRepository.Table;

            var searchParams = EnsureSearchQueryIsValid(queryParams, ResolveSearchQuery);
            if (searchParams != null)
            {
                query = query.HandleSearchParams(searchParams);
            }

            query = query.GetQueryDynamic(sortColumn, @descending);

            var _ = new SearchWrapper<TransactionDto, Transaction>();
            return count
                ? _.ToCount(query)
                : _.ToList(query, page, limit,
                    list => list.Select(entity => entity.ToDto()).ToList() as IList<TransactionDto>);
        }

        public int GetTransactionsCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null)
        {
            var query = GetTransactionQuery(createdAtMin, createdAtMax);

            return query.Count();
        }

        private IQueryable<Transaction> GetTransactionQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null, IList<int> ids = null)
        {
            var query = _transactionRepository.Table;

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