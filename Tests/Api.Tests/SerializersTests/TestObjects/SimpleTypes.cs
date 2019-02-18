using Newtonsoft.Json;

namespace Api.Tests.SerializersTests.TestObjects
{
    /// <summary>
    /// Represent test object with simple types
    /// </summary>
    public class SimpleTypes
    {
        [JsonProperty("first_property")]
        public string FirstProperty { get; set; }

        [JsonProperty("second_property")]
        public string SecondProperty { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is SimpleTypes that)
            {
                return that.FirstProperty.Equals(FirstProperty) && that.SecondProperty.Equals(SecondProperty);
            }

            return false;
        }
    }
}