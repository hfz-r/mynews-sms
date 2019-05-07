using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public interface ISupplierApiService
    {
        SupplierMaster GetSupplierById(int id);
        IList<SupplierMaster> GetSuppliers(int limit = 50, int page = 1, int sinceId = 0);
        int GetSuppliersCount(int limit = 50, int page = 1, int sinceId = 0);
    }
}