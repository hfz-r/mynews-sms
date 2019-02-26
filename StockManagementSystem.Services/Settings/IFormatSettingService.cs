using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Settings;

namespace StockManagementSystem.Services.Settings
{
    public interface IFormatSettingService
    {
        void DeleteShelfLocationFormat(FormatSetting shelfLocationFormat);
        Task<IList<FormatSetting>> GetAllBarcodeFormatsAsync();
        Task<IList<FormatSetting>> GetAllShelfLocationFormatsAsync();
        Task<FormatSetting> GetBarcodeFormatByIdAsync(int barcodeFormatId);
        Task<FormatSetting> GetShelfLocationFormatByIdAsync(int shelflocationFormatId);
        Task InsertShelfLocationFormat(FormatSetting shelfLocationFormat);
        void UpdateBarcodeFormat(FormatSetting barcodeFormat);
        void UpdateShelfLocationFormat(FormatSetting shelfLocationFormat);
        Task<FormatSetting> GetBarcodeFormatBySeqAsync(int? seqNo);
    }
}