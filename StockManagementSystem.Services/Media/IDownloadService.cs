using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using StockManagementSystem.Core.Domain.Media;

namespace StockManagementSystem.Services.Media
{
    public interface IDownloadService
    {
        Task DeleteDownload(Download download);
        Task<byte[]> GetDownloadBits(XElement element, CancellationToken cancellationToken);
        Task<Download> GetDownloadById(int downloadId);
        Task InsertDownload(Download download);
        Task UpdateDownload(Download download);
    }
}