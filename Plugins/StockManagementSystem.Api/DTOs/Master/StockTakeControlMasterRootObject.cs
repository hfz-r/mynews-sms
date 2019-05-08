using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class StockTakeControlMasterRootObject : ISerializableObject
    {
        public StockTakeControlMasterRootObject()
        {
            StockTakeControlMaster = new List<StockTakeControlMasterDto>();
        }

        [JsonProperty("stockTakeControl")]
        public IList<StockTakeControlMasterDto> StockTakeControlMaster { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "stockTakeControl";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(StockTakeControlMasterDto);
        }
    }
}
