using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class ASNHeaderMasterRootObject : ISerializableObject
    {
        public ASNHeaderMasterRootObject()
        {
            ASNHeaderMaster = new List<ASNHeaderMasterDto>();
        }

        [JsonProperty("asnHeader")]
        public IList<ASNHeaderMasterDto> ASNHeaderMaster { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "asnHeader";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ASNHeaderMasterDto);
        }
    }
}
