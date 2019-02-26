using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StockManagementSystem.Api.DTOs.Items
{
    public class ItemsRootObject : ISerializableObject
    {
        public ItemsRootObject()
        {
            Items = new List<ItemDto>();
        }

        [JsonProperty("items")]
        public IList<ItemDto> Items { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "items";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ItemDto);
        }
    }
}