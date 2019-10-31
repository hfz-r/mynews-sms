using System;
using System.Collections.Generic;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Api.Json.Extensions;

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
            var typeName = typeof(T).Name;
            var normalizedName = GenericPropertyHelper.GetGenericProperty(typeName);

            return !string.IsNullOrEmpty(normalizedName)
                //? normalizedName.GetNormalizedPropertyName()
                ? string.Concat(normalizedName, "s").ToLowerInvariant()
                : throw new InvalidOperationException($"{typeName} is not a valid context.");
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(T);
        }
    }
}