using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockManagementSystem.Services.Settings
{
    public class FormatSettingService : IFormatSettingService
    {
        private readonly IRepository<FormatSetting> _formatSettingRepository;

        public FormatSettingService(
            IRepository<FormatSetting> formatSettingRepository)
        {
            _formatSettingRepository = formatSettingRepository;
        }

        public async Task<IList<FormatSetting>> GetAllShelfLocationFormatsAsync()
        {
            var query = _formatSettingRepository.Table;

            query = query.Where(u => u.Format.Contains("Shelf")).OrderBy(c => c.Id);

            return await query.ToListAsync();
        }

        public virtual void UpdateShelfLocationFormat(FormatSetting shelfLocationFormat)
        {
            if (shelfLocationFormat == null)
                throw new ArgumentNullException(nameof(shelfLocationFormat));

            _formatSettingRepository.Update(shelfLocationFormat);
        }

        public virtual void DeleteShelfLocationFormat(FormatSetting shelfLocationFormat)
        {
            if (shelfLocationFormat == null)
                throw new ArgumentNullException(nameof(shelfLocationFormat));

            _formatSettingRepository.Delete(shelfLocationFormat);
        }

        public async Task InsertShelfLocationFormat(FormatSetting shelfLocationFormat)
        {
            if (shelfLocationFormat == null)
                throw new ArgumentNullException(nameof(shelfLocationFormat));

            await _formatSettingRepository.InsertAsync(shelfLocationFormat);
        }

        public async Task<IList<FormatSetting>> GetAllBarcodeFormatsAsync()
        {
            var query = _formatSettingRepository.Table;

            query = query.Where(u => u.Format.Contains("Barcode")).OrderBy(c => c.Id);

            return await query.ToListAsync();
        }
        
        public virtual void UpdateBarcodeFormat(FormatSetting barcodeFormat)
        {
            if (barcodeFormat == null)
                throw new ArgumentNullException(nameof(barcodeFormat));

            _formatSettingRepository.Update(barcodeFormat);
        }
        

        #region Identity

        public async Task<FormatSetting> GetShelfLocationFormatByIdAsync(int shelflocationFormatId)
        {
            if (shelflocationFormatId == 0)
                throw new ArgumentNullException(nameof(shelflocationFormatId));

            var shelflocationFormat = await _formatSettingRepository.Table.FirstOrDefaultAsync(u => u.Id == shelflocationFormatId);
            return shelflocationFormat;
        }

        public async Task<FormatSetting> GetBarcodeFormatByIdAsync(int barcodeFormatId)
        {
            if (barcodeFormatId == 0)
                throw new ArgumentNullException(nameof(barcodeFormatId));

            var barcodeFormat = await _formatSettingRepository.Table.FirstOrDefaultAsync(u => u.Id == barcodeFormatId);
            return barcodeFormat;
        }

        #endregion
    }
}
