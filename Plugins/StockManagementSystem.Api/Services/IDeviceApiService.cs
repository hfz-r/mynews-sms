using System;
using System.Collections.Generic;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DTOs.Devices;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Core.Domain.Devices;

namespace StockManagementSystem.Api.Services
{
    public interface IDeviceApiService
    {
        IList<Device> GetDevices(IList<int> ids = null, int limit = 50, int page = 1, int sinceId = 0,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, string status = "");

        Device GetDeviceById(int id);

        Device GetDeviceBySerialNo(string serialNo);

        int GetDevicesCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, string status = "");

        Search<DeviceDto> Search(
            string queryParams = "",
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false,
            bool count = false);
    }
}