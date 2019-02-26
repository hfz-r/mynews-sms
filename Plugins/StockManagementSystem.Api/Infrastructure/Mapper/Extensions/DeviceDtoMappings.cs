using StockManagementSystem.Api.DTOs.Devices;
using StockManagementSystem.Core.Domain.Devices;

namespace StockManagementSystem.Api.Infrastructure.Mapper.Extensions
{
    public static class DeviceDtoMappings
    {
        public static DeviceDto ToDto(this Device device)
        {
            return device.MapTo<Device, DeviceDto>();
        }
    }
}