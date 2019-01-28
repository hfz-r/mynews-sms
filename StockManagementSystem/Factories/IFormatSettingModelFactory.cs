using System.Threading.Tasks;
using StockManagementSystem.Models.Setting;

namespace StockManagementSystem.Factories
{
    public interface IFormatSettingModelFactory
    {
        Task<BarcodeListModel> PrepareBarcodeFormatListModel(BarcodeSearchModel searchModel);
        Task<BarcodeSearchModel> PrepareBarcodeFormatSearchModel(BarcodeSearchModel searchModel);
        Task<FormatSettingContainerModel> PrepareFormatSettingContainerModel(FormatSettingContainerModel formatSettingContainerModel);
        Task<ShelfListModel> PrepareShelfFormatListModel(ShelfSearchModel searchModel);
        Task<ShelfSearchModel> PrepareShelfFormatSearchModel(ShelfSearchModel searchModel);
    }
}