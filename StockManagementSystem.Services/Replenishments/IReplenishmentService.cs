using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Settings;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Services.Replenishments
{
    public interface IReplenishmentService
    {
        void DeleteReplenishment(Replenishment replenishment);
        void DeleteReplenishmentStore(int Id, Store store);
        Task<ICollection<Replenishment>> GetAllReplenishmentsAsync();
        Task<Replenishment> GetReplenishmentByIdAsync(int replenishmentId);
        Task<IPagedList<Replenishment>> GetReplenishmentsAsync(int[] storeIds = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);
        Task InsertReplenishment(Replenishment replenishment);
        void UpdateReplenishment(Replenishment replenishment);
    }
}