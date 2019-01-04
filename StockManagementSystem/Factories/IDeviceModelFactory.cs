using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Models.Devices;

namespace StockManagementSystem.Factories
{
    public interface IDeviceModelFactory
    {
        Task<DeviceModel> PrepareDeviceModel(DeviceModel model, Device device);
        Task<DeviceSearchModel> PrepareDeviceSearchModel(DeviceSearchModel searchModel);
        Task<DeviceListModel> PrepareDeviceListModel(DeviceSearchModel searchModel);
        Task<DeviceModel> PrepareDeviceListModel();
    }
}