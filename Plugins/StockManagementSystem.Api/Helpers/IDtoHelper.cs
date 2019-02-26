using StockManagementSystem.Api.DTOs.Devices;
using StockManagementSystem.Core.Domain.Devices;

namespace StockManagementSystem.Api.Helpers
{
    public interface IDtoHelper
    {
        DeviceDto PrepareDeviceDto(Device device);
    }
}