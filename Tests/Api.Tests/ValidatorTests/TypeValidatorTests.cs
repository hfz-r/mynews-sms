using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using StockManagementSystem.Api.Validators;

namespace Api.Tests.ValidatorTests
{
    [TestFixture]
    public class TypeValidatorTests
    {
        [Test]
        [SetCulture("de-de")]
        public void Should_validate_decimal_properly_when_current_culture_uses_comma_as_decimal_point()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            properties.Add("decimal_number", 33.33);

            var validator = new TypeValidator<TestModelDto>();

            bool result = validator.IsValid(properties);

            Assert.IsTrue(result);
        }
    }

    internal class TestModelDto
    {
        [JsonProperty("decimal_number")]
        public decimal? SomeDecimalNumber { get; set; }
    }
}