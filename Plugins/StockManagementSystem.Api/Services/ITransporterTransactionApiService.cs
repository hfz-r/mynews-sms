using System;
using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Transactions;

namespace StockManagementSystem.Api.Services
{
    public interface ITransporterTransactionApiService
    {
        IList<TransporterTransaction> GetTransporterTransaction(IList<int> ids = null, int limit = 50, int page = 1, int sinceId = 0, DateTime? createdAtMin = null, DateTime? createdAtMax = null);

        TransporterTransaction GetTransporterTransactionById(int id);

        int GetTransporterTransactionCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null);
    }
}