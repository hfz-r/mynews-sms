using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.ShelfLocation
{
    public class ShelfLocationRootObject : ISerializableObject
    {
        public ShelfLocationRootObject()
        {
            ShelfLocation = new List<ShelfLocationDto>();
        }

        [JsonProperty("shelf_location")]
        public IList<ShelfLocationDto> ShelfLocation { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "shelf_location";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ShelfLocationDto);
        }
    }
}