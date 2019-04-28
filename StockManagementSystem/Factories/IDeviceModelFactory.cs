using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Models.Devices;
using StockManagementSystem.Models.Setting;

namespace StockManagementSystem.Factories
{
    public interface IDeviceModelFactory
    {
        Task<DeviceModel> PrepareDeviceModel(DeviceModel model, Device device);
        Task<DeviceSearchModel> PrepareDeviceSearchModel(DeviceSearchModel searchModel);
        Task<DeviceListModel> PrepareDeviceListModel(DeviceSearchModel searchModel);
        Task<DeviceModel> PrepareDeviceListbyStoreModel(int storeID);
        Task<DeviceModel> PrepareDeviceListModel();
        Task<DeviceTrackingContainerModel> PrepareDeviceTrackingContainerModel(DeviceTrackingContainerModel deviceTrackingContainerModel);
        Task<MapDeviceListModel> PrepareMapDeviceListingModel(MapDeviceSearchModel searchModel);

    }
}