using System;
using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Transactions;

namespace StockManagementSystem.Api.Services
{
    public interface ITransactionApiService
    {
        Transaction GetTransactionById(int id);

        IList<Transaction> GetTransactions(IList<int> ids = null, int limit = 50, int page = 1, int sinceId = 0, DateTime? createdAtMin = null, DateTime? createdAtMax = null);

        int GetTransactionsCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null);
    }
}