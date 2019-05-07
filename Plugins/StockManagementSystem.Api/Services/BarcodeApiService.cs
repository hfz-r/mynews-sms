using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public class BarcodeApiService : IBarcodeApiService
    {
        private readonly IRepository<BarcodeMaster> _barcodeRepository;

        public BarcodeApiService(IRepository<BarcodeMaster> barcodeRepository)
        {
            _barcodeRepository = barcodeRepository;
        }

        public IList<BarcodeMaster> GetBarcodes(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetBarcodesQuery(sinceId);

            return new ApiList<BarcodeMaster>(query, page - 1, limit);
        }

        public BarcodeMaster GetBarcodeById(int id)
        {
            if (id <= 0)
                return null;

            var barcode = _barcodeRepository.Table.FirstOrDefault(i => i.Id == id);

            return barcode;
        }

        public int GetBarcodesCount(int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            return GetBarcodesQuery(sinceId).Count();
        }

        private IQueryable<BarcodeMaster> GetBarcodesQuery(int sinceId = 0)
        {
            var query = _barcodeRepository.Table;

            if (sinceId > 0)
                query = query.Where(i => i.Id > sinceId);

            query = query.OrderBy(i => i.Id);

            return query;
        }
    }
}