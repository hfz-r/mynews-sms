using System;
using System.Collections.Generic;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DTOs.TransporterTransaction;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Core.Domain.Transactions;

namespace StockManagementSystem.Api.Services
{
    public interface ITransporterTransactionApiService
    {
        IList<TransporterTransaction> GetTransporterTransaction(IList<int> ids = null, int limit = 50, int page = 1, int sinceId = 0, DateTime? createdAtMin = null, DateTime? createdAtMax = null);

        TransporterTransaction GetTransporterTransactionById(int id);

        int GetTransporterTransactionCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null);

        Search<TransporterTransactionDto> Search(
            string queryParams = "",
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false,
            bool count = false);
    }
}