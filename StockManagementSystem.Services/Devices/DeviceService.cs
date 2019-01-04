using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.Stores;

namespace StockManagementSystem.Services.Devices
{
    public class DeviceService : IDeviceService
    {
        private readonly IRepository<Device> _deviceRepository;
        private readonly IRepository<Store> _storeRepository;

        public DeviceService(
            IRepository<Device> deviceRepository,
            IRepository<Store> storeRepository)
        {
            _deviceRepository = deviceRepository;
            _storeRepository = storeRepository;
        }

        public async Task<Device> GetDeviceBySerialNoAsync(string serialNo)
        {
            if (serialNo == null)
                throw new ArgumentNullException(nameof(serialNo));

            var device = await _deviceRepository.Table.FirstOrDefaultAsync(u => u.SerialNo == serialNo);
            return device;
        }

        public Task<IPagedList<Device>> GetDevicesAsync(
            int[] storeIds = null,
            string serialNo = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool getOnlyTotalCount = false)
        {
            var query = _deviceRepository.Table;

            if (storeIds != null && storeIds.Length > 0)
            {
                query = query.Join(_storeRepository.Table, x => x.StoreId, y => y.P_BranchNo,
                        (x, y) => new { Device = x, Store = y })
                    .Where(z => storeIds.Contains(z.Store.P_BranchNo))
                    .Select(z => z.Device)
                    .Distinct();
            }
            if (!string.IsNullOrWhiteSpace(serialNo))
                query = query.Where(u => u.SerialNo.Contains(serialNo));

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return Task.FromResult<IPagedList<Device>>(new PagedList<Device>(query, pageIndex, pageSize,
                getOnlyTotalCount));
        }

        public virtual void UpdateDevice(Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            _deviceRepository.Update(device);
        }

        public virtual void DeleteDevice(Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            _deviceRepository.Delete(device);
        }

        public async Task InsertDevice(Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            await _deviceRepository.InsertAsync(device);
        }

        #region Identity 

        public async Task<Device> GetDeviceByIdAsync(int deviceId)
        {
            if (deviceId == 0)
                throw new ArgumentNullException(nameof(deviceId));

            var device = await _deviceRepository.Table.FirstOrDefaultAsync(u => u.Id == deviceId);
            return device;
        }

        public async Task SetSerialNo(Device device, string newSerialNo)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            if (newSerialNo == null)
                throw new Exception("Serial number cannot be null");

            newSerialNo = newSerialNo.Trim();
            var oldSerialNo = device.SerialNo;

            var device2 = await _deviceRepository.Table.FirstOrDefaultAsync(u => u.SerialNo == newSerialNo);            
            if (device2 != null && device.SerialNo!= device2.SerialNo)
                throw new Exception("The Serial Number is already in use");

            device.SerialNo = newSerialNo;

            if (string.IsNullOrEmpty(oldSerialNo) ||
                oldSerialNo.Equals(newSerialNo, StringComparison.InvariantCultureIgnoreCase))
                return;
        }

        #endregion
    }
}