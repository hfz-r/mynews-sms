using System.Collections.Generic;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Devices;

namespace StockManagementSystem.Services.Devices
{
    public interface IDeviceService
    {
        void DeleteDevice(Device device);

        Task<Device> GetDeviceBySerialNoAsync(string serialNo);

        Task<IPagedList<Device>> GetDevicesAsync(
            int[] storeIds = null,
            string serialNo = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool getOnlyTotalCount = false);

        Task<ICollection<Device>> GetAllDevicesAsync();

        void UpdateDevice(Device device);

        Task InsertDevice(Device device);

        Task<Device> GetDeviceByIdAsync(int deviceId);

        Task SetSerialNo(Device device, string newSerialNo);


    }
}