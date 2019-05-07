using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public class ShiftControlApiService : IShiftControlApiService
    {
        private readonly IRepository<ShiftControlMaster> _shiftControlRepository;

        public ShiftControlApiService(IRepository<ShiftControlMaster> shiftControlRepository)
        {
            _shiftControlRepository = shiftControlRepository;
        }

        public IList<ShiftControlMaster> GetShiftControls(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetShiftControlsQuery(sinceId);

            return new ApiList<ShiftControlMaster>(query, page - 1, limit);
        }

        public ShiftControlMaster GetShiftControlById(int id)
        {
            if (id <= 0)
                return null;

            var shiftControl = _shiftControlRepository.Table.FirstOrDefault(i => i.Id == id);

            return shiftControl;
        }

        public int GetShiftControlsCount(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetShiftControlsQuery(sinceId).Count();
        }

        private IQueryable<ShiftControlMaster> GetShiftControlsQuery(int sinceId = 0)
        {
            var query = _shiftControlRepository.Table;

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}