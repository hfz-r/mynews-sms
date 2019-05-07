using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public class ASNHeaderApiService : IASNHeaderApiService
    {
        private readonly IRepository<ASNHeaderMaster> _asnHeaderRepository;

        public ASNHeaderApiService(IRepository<ASNHeaderMaster> asnHeaderRepository)
        {
            _asnHeaderRepository = asnHeaderRepository;
        }

        public IList<ASNHeaderMaster> GetASNHeaders(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetASNHeadersQuery(sinceId);

            return new ApiList<ASNHeaderMaster>(query, page - 1, limit);
        }

        public ASNHeaderMaster GetASNHeaderById(int id)
        {
            if (id <= 0)
                return null;

            var asnHeader = _asnHeaderRepository.Table.FirstOrDefault(i => i.Id == id);

            return asnHeader;
        }

        public int GetASNHeadersCount(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetASNHeadersQuery(sinceId).Count();
        }

        private IQueryable<ASNHeaderMaster> GetASNHeadersQuery(int sinceId = 0)
        {
            var query = _asnHeaderRepository.Table;

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}