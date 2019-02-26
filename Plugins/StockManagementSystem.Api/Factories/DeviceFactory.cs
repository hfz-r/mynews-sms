using System;
using StockManagementSystem.Core.Domain.Devices;

namespace StockManagementSystem.Api.Factories
{
    public class DeviceFactory : IFactory<Device>
    {
        public Device Initialize()
        {
            var defaultDevice = new Device()
            {
                Status = "0",
            };

            return defaultDevice;
        }
    }
}