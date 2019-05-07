using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public class ASNDetailApiService : IASNDetailApiService
    {
        private readonly IRepository<ASNDetailMaster> _asnDetailRepository;

        public ASNDetailApiService(IRepository<ASNDetailMaster> asnDetailRepository)
        {
            _asnDetailRepository = asnDetailRepository;
        }

        public IList<ASNDetailMaster> GetASNDetails(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetASNDetailsQuery(sinceId);

            return new ApiList<ASNDetailMaster>(query, page - 1, limit);
        }

        public ASNDetailMaster GetASNDetailById(int id)
        {
            if (id <= 0)
                return null;

            var asnDetail = _asnDetailRepository.Table.FirstOrDefault(i => i.Id == id);

            return asnDetail;
        }

        public int GetASNDetailsCount(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetASNDetailsQuery(sinceId).Count();
        }

        private IQueryable<ASNDetailMaster> GetASNDetailsQuery(int sinceId = 0)
        {
            var query = _asnDetailRepository.Table;

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}