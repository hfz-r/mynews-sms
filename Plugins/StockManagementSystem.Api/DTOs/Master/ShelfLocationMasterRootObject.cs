using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.DTOs.Master
{
    public class ShelfLocationMasterRootObject : ISerializableObject
    {
        public ShelfLocationMasterRootObject()
        {
            ShelfLocationMaster = new List<ShelfLocationMasterDto>();
        }

        [JsonProperty("shelfLocation")]
        public IList<ShelfLocationMasterDto> ShelfLocationMaster { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "shelfLocation";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ShelfLocationMasterDto);
        }
    }
}
