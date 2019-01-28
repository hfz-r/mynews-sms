using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Setting
{
    public class FormatSettingContainerModel : BaseModel
    {

        public FormatSettingContainerModel()
        {
            ListShelfFormat = new ShelfSearchModel();
            ListBarcodeFormat = new BarcodeSearchModel();
        }

        public ShelfSearchModel ListShelfFormat { get; set; }

        public BarcodeSearchModel ListBarcodeFormat { get; set; }
    }
}