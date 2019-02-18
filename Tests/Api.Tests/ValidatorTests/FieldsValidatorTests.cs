using Api.Tests.SerializersTests.TestObjects;
using NUnit.Framework;
using StockManagementSystem.Api.Validators;

namespace Api.Tests.ValidatorTests
{
    [TestFixture]
    public class FieldsValidatorTests
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Should_return_empty_dictionary_when_empty_fields_parameter_passed(string emptyFields)
        {
            var validator = new FieldsValidator();

            var result = validator.GetValidFields(emptyFields, typeof(SimpleTypes));

            Assert.IsEmpty(result);
        }

        [Test]
        [TestCase("first_property")]
        [TestCase("second_property")]
        [TestCase("first_property,second_property")]
        [TestCase("firstproperty,secondproperty")]
        [TestCase("firstProperty,secondproperty")]
        public void Should_return_non_empty_dictionary_when_only_valid_fields_parameters_passed(string validFields)
        {
            var validator = new FieldsValidator();

            var result = validator.GetValidFields(validFields, typeof(SimpleTypes));

            Assert.IsNotEmpty(result);
        }

        [Test]
        [TestCase("first_property,second_property")]
        [TestCase("firstproperty,secondproperty")]
        [TestCase("firstProperty,Secondproperty")]
        public void Should_return_dictionary_contains_valid_fields_when_valid_fields_parameters_passed(string validFields)
        {
            var validator = new FieldsValidator();

            var result = validator.GetValidFields(validFields, typeof(SimpleTypes));

            Assert.True(result.ContainsKey("firstproperty"));
            Assert.True(result.ContainsKey("secondproperty"));
        }

        [Test]
        [TestCase("FiRst_PropertY,second_property")]
        [TestCase("firstproperty,SecondProPerty")]
        [TestCase("firstProperty,Secondproperty")]
        public void Should_return_dictionary_contains_valid_fields_with_lowercase_when_valid_fields_parameters_passed(string validFields)
        {
            var validator = new FieldsValidator();

            var result = validator.GetValidFields(validFields, typeof(SimpleTypes));

            Assert.True(result.ContainsKey("firstproperty"));
            Assert.True(result.ContainsKey("secondproperty"));
        }

        [Test]
        [TestCase("first_property")]
        public void Should_return_dictionary_contains_valid_fields_without_underscores_when_valid_fields_parameters_passed(string validFields)
        {
            var validator = new FieldsValidator();

            var result = validator.GetValidFields(validFields, typeof(SimpleTypes));

            Assert.True(result.ContainsKey("firstproperty"));
        }

        [Test]
        [TestCase("first_property,second_property,invalid")]
        [TestCase("firstproperty,secondproperty,invalid")]
        [TestCase("firstProperty,Secondproperty,invalid")]
        public void Should_return_dictionary_with_valid_fields_when_valid_and_invalid_fields_parameters_passed(string mixedFields)
        {
            var validator = new FieldsValidator();

            var result = validator.GetValidFields(mixedFields, typeof(SimpleTypes));

            Assert.AreEqual(2, result.Count);
            Assert.True(result.ContainsKey("firstproperty"));
            Assert.True(result.ContainsKey("secondproperty"));
        }

        [Test]
        [TestCase("invalid")]
        [TestCase("multiple,invalid,fields")]
        public void Should_return_empty_dictionary_when_invalid_fields_parameters_passed(string invalidFields)
        {
            var validator = new FieldsValidator();

            var result = validator.GetValidFields(invalidFields, typeof(SimpleTypes));

            Assert.IsEmpty(result);
        }

        [Test]
        [TestCase("invalid,,")]
        [TestCase(",,,*multiple,)in&^valid,f@#%$ields+_-,,,,,")]
        [TestCase(".")]
        [TestCase(",")]
        [TestCase("()")]
        [TestCase("'\"\"")]
        [TestCase(",,,,mail, 545, ''\"")]
        [TestCase("peshoid")]
        public void Should_return_empty_dictionary_when_invalid_fields_with_symbols_parameters_passed(string invalidFields)
        {
            var validator = new FieldsValidator();

            var result = validator.GetValidFields(invalidFields, typeof(SimpleTypes));

            Assert.IsEmpty(result);
        }
    }
}