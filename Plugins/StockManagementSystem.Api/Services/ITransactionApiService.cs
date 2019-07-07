using System;
using System.Collections.Generic;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DTOs.Transactions;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Core.Domain.Transactions;

namespace StockManagementSystem.Api.Services
{
    public interface ITransactionApiService
    {
        Transaction GetTransactionById(int id);

        Search<TransactionDto> Search(
            string queryParams = "",
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false,
            bool count = false);

        IList<Transaction> GetTransactions(
            IList<int> ids = null,
            int limit = 50,
            int page = 1, int sinceId = 0,
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null);

        int GetTransactionsCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null);
    }
}