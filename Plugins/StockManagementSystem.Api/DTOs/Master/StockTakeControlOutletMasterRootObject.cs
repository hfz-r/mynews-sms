using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class StockTakeControlOutletMasterRootObject : ISerializableObject
    {
        public StockTakeControlOutletMasterRootObject()
        {
            StockTakeControlOutletMaster = new List<StockTakeControlOutletMasterDto>();
        }

        [JsonProperty("stockTakeControlOutlet")]
        public IList<StockTakeControlOutletMasterDto> StockTakeControlOutletMaster { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "stockTakeControlOutlet";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(StockTakeControlOutletMasterDto);
        }
    }
}
