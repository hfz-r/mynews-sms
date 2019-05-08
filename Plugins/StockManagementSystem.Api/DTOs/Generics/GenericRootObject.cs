using System;
using System.Collections.Generic;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.DTOs.Generics
{
    public class GenericRootObject<T> : ISerializableObject where T : BaseDto
    {
        public GenericRootObject()
        {
            Entities = new List<T>();
        }

        [JsonPropertyGenericTypeName(0)]
        public IList<T> Entities { get; set; }

        public string GetPrimaryPropertyName()
        {
            return typeof(T).Name.Remove(typeof(T).Name.Length - 3).ToLowerInvariant();
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(T);
        }
    }
}