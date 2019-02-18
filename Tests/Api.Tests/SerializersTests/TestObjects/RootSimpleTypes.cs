using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using StockManagementSystem.Api.DTOs;

namespace Api.Tests.SerializersTests.TestObjects
{
    /// <summary>
    /// Represent root object for the simple types
    /// </summary>
    public class RootSimpleTypes : ISerializableObject
    {
        public RootSimpleTypes()
        {
            Items = new List<SimpleTypes>();
        }

        [JsonProperty("primary_property")]
        public IList<SimpleTypes> Items { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "primary_property";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(SimpleTypes);
        }
    }
}