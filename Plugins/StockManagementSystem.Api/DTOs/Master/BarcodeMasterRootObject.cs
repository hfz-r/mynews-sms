using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class BarcodeMasterRootObject : ISerializableObject
    {
        public BarcodeMasterRootObject()
        {
            BarcodeMaster = new List<BarcodeMasterDto>();
        }

        [JsonProperty("barcode")]
        public IList<BarcodeMasterDto> BarcodeMaster { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "barcode";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(BarcodeMasterDto);
        }
    }
}
