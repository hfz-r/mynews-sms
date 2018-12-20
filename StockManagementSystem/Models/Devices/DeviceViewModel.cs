using StockManagementSystem.Core.Domain.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Models.Devices
{
    public class DeviceViewModel
    {
        public IEnumerable<Device> Device { get; set; }
    }
}
