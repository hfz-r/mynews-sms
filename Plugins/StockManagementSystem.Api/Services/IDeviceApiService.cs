using System;
using System.Collections.Generic;
using StockManagementSystem.Core.Domain.Devices;

namespace StockManagementSystem.Api.Services
{
    public interface IDeviceApiService
    {
        IList<Device> GetDevices(IList<int> ids = null, int limit = 50, int page = 1, int sinceId = 0,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, string status = "");

        Device GetDeviceById(int id);

        int GetDevicesCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, string status = "");
    }
}