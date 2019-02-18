using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using StockManagementSystem.Api.DTOs;

namespace Api.Tests.SerializersTests.TestObjects
{
    /// <summary>
    /// Represent root object for the complex types
    /// </summary>
    public class RootComplexTypes : ISerializableObject
    {
        public RootComplexTypes()
        {
            Items = new List<ComplexTypes>();
        }

        [JsonProperty("primary_complex_property")]
        public IList<ComplexTypes> Items { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "primary_complex_property";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(ComplexTypes);
        }
    }
}