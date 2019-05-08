using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class SupplierMasterRootObject : ISerializableObject
    {
        public SupplierMasterRootObject()
        {
            SupplierMaster = new List<SupplierMasterDto>();
        }

        [JsonProperty("supplier")]
        public IList<SupplierMasterDto> SupplierMaster { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "supplier";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(SupplierMasterDto);
        }
    }
}
