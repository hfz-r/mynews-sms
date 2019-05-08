using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class ASNDetailMasterRootObject : ISerializableObject
    {
        public ASNDetailMasterRootObject()
        {
            ASNDetailMaster = new List<ASNDetailMasterDto>();
        }

        [JsonProperty("asnDetail")]
        public IList<ASNDetailMasterDto> ASNDetailMaster { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "asnDetail";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ASNDetailMasterDto);
        }
    }
}
