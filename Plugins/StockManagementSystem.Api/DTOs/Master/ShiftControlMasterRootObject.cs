using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class ShiftControlMasterRootObject : ISerializableObject
    {
        public ShiftControlMasterRootObject()
        {
            ShiftControlMaster = new List<ShiftControlMasterDto>();
        }

        [JsonProperty("shiftControl")]
        public IList<ShiftControlMasterDto> ShiftControlMaster { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "shiftControl";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ShiftControlMasterDto);
        }
    }
}
