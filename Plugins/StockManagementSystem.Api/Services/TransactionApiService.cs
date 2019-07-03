using System;
using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Transactions;

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

        public IEnumerable<Transaction> GetTransactionByBranchNo(int branchNo, DateTime? createdAtMin = null, DateTime? createdAtMax = null)
        {
            var transactions = GetTransactionQuery(createdAtMin, createdAtMax);

            var query = from t in transactions
                where t.P_BranchNo == branchNo
                select t;

            return query.AsEnumerable();
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