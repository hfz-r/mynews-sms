using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Master;

namespace StockManagementSystem.Api.Services
{
    public interface IBarcodeApiService
    {
        BarcodeMaster GetBarcodeById(int id);
        IList<BarcodeMaster> GetBarcodes(int limit = 50, int page = 1, int sinceId = 0);
        int GetBarcodesCount(int limit = 50, int page = 1, int sinceId = 0);
    }
}