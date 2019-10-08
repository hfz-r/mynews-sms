using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Media;
using StockManagementSystem.Services.Events;

namespace StockManagementSystem.Services.Media
{
    public class DownloadService : IDownloadService
    {
        private readonly IEventPublisher _eventPubisher;
        private readonly IRepository<Download> _downloadRepository;

        public DownloadService(IEventPublisher eventPubisher, IRepository<Download> downloadRepository)
        {
            _eventPubisher = eventPubisher;
            _downloadRepository = downloadRepository;
        }

        public async Task<Download> GetDownloadById(int downloadId)
        {
            if (downloadId == 0)
                return null;

            return await _downloadRepository.GetByIdAsync(downloadId);
        }

        public async Task DeleteDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException(nameof(download));

            await _downloadRepository.DeleteAsync(download);

            _eventPubisher.EntityDeleted(download);
        }

        public async Task InsertDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException(nameof(download));

            await _downloadRepository.InsertAsync(download);

            _eventPubisher.EntityInserted(download);
        }

        public async Task UpdateDownload(Download download)
        {
            if (download == null)
                throw new ArgumentNullException(nameof(download));

            await _downloadRepository.UpdateAsync(download);

            _eventPubisher.EntityUpdated(download);
        }

        public async Task<byte[]> GetDownloadBits(XElement element, CancellationToken cancellationToken)
        {
            using (var ms = new MemoryStream())
            {
                await element.SaveAsync(ms, SaveOptions.None, cancellationToken);
                var fileBytes = ms.ToArray();
                return fileBytes;
            }
        }
    }
}