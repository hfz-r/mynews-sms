using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Devices
{
    public class DevicesRootObject : ISerializableObject
    {
        public DevicesRootObject()
        {
            Devices = new List<DeviceDto>();
        }

        [JsonProperty("devices")]
        public IList<DeviceDto> Devices { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "devices";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(DeviceDto);
        }
    }
}