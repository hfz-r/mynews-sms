using StockManagementSystem.Models.Devices;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Setting
{
    public class DeviceTrackingContainerModel : BaseModel
    {
        public DeviceTrackingContainerModel()
        {
            DeviceMap = new DeviceModel();
            DeviceListing = new MapDeviceSearchModel();
        }

        public DeviceModel DeviceMap { get; set; }

        public MapDeviceSearchModel DeviceListing { get; set; }
    }
}