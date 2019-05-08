using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class StockTakeRightMasterRootObject : ISerializableObject
    {
        public StockTakeRightMasterRootObject()
        {
            StockTakeRightMaster = new List<StockTakeRightMasterDto>();
        }

        [JsonProperty("stockTakeRight")]
        public IList<StockTakeRightMasterDto> StockTakeRightMaster { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "stockTakeRight";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(StockTakeRightMasterDto);
        }
    }
}
