using System.Collections.Generic;
using Newtonsoft.Json;

namespace Api.Tests.SerializersTests.TestObjects
{
    /// <summary>
    /// Represent test object with complex types
    /// </summary>
    public class ComplexTypes
    {
        [JsonProperty("string_property")]
        public string StringProperty { get; set; }

        [JsonProperty("int_property")]
        public int IntProperty { get; set; }

        [JsonProperty("bool_property")]
        public bool BoolProperty { get; set; }

        [JsonProperty("list_of_simple_types")]
        public IList<SimpleTypes> ListOfSimpleTypes { get; set; }

        [JsonProperty("simple_types")]
        public SimpleTypes SimpleTypes { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ComplexTypes)
            {
                var that = obj as ComplexTypes;

                return that.StringProperty.Equals(StringProperty) &&
                       that.IntProperty == IntProperty &&
                       that.BoolProperty == BoolProperty &&
                       that.ListOfSimpleTypes.Equals(ListOfSimpleTypes) &&
                       that.SimpleTypes.Equals(SimpleTypes);
            }

            return false;
        }
    }
}