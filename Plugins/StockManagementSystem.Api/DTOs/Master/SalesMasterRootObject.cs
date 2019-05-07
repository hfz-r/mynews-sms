using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class SalesMasterRootObject : ISerializableObject
    {
        public SalesMasterRootObject()
        {
            SalesMaster = new List<SalesMasterDto>();
        }

        [JsonProperty("sales")]
        public IList<SalesMasterDto> SalesMaster { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "sales";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(SalesMasterDto);
        }
    }
}
