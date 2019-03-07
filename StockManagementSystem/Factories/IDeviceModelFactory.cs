using System.Threading.Tasks;
using StockManagementSystem.Models.Devices;
using StockManagementSystem.Models.Setting;

namespace StockManagementSystem.Factories
{
    public interface IDeviceModelFactory
    {
        Task<DeviceSearchModel> PrepareDeviceSearchModel(DeviceSearchModel searchModel);

        Task<DeviceListModel> PrepareDeviceListModel(DeviceSearchModel searchModel);

        Task<DeviceModel> PrepareDeviceListModel();

        Task<DeviceTrackingContainerModel> PrepareDeviceTrackingContainerModel(DeviceTrackingContainerModel deviceTrackingContainerModel);

        Task<MapDeviceListModel> PrepareMapDeviceListingModel(MapDeviceSearchModel searchModel);
    }
}