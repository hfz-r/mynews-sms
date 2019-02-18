using System;
using System.Collections.Generic;
using Api.Tests.SerializersTests.TestObjects;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using StockManagementSystem.Api.DTOs;
using StockManagementSystem.Api.Json.Serializer;

namespace Api.Tests.SerializersTests
{
    [TestFixture]
    public class JsonFieldsSerializerTests
    {
        #region With simple type test object

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Should_serialize_everything_from_the_passed_object_when_empty_fields_parameter_passed(string emptyFieldsParameter)
        {
            IJsonFieldsSerializer serializer = new JsonFieldsSerializer();

            var serializableObject = new RootSimpleTypes();
            serializableObject.Items.Add(new SimpleTypes
            {
                FirstProperty = "first property value",
                SecondProperty = "second property value"
            });

            string serializedObjectJson = serializer.Serialize(serializableObject, emptyFieldsParameter);

            var serializableObjectFromJson = JsonConvert.DeserializeObject<RootSimpleTypes>(serializedObjectJson);

            Assert.AreEqual(serializableObject.Items.Count, serializableObjectFromJson.Items.Count);
            Assert.AreEqual(serializableObject.Items[0], serializableObjectFromJson.Items[0]);
            Assert.AreEqual("first property value", serializableObjectFromJson.Items[0].FirstProperty);
            Assert.AreEqual("second property value", serializableObjectFromJson.Items[0].SecondProperty);
        }

        [Test]
        public void Should_throw_null_exception_when_null_object_passed()
        {
            Assert.Throws<ArgumentNullException>(WhenNullObjectToSerializedPassed);
        }

        [Test]
        public void Should_return_empty_json_when_no_valid_fields_in_fields_parameter_passed()
        {
            var serializableObject = new RootSimpleTypes();
            serializableObject.Items.Add(new SimpleTypes()
            {
                FirstProperty = "first property value",
                SecondProperty = "second property value",
            });

            IJsonFieldsSerializer serializer = new JsonFieldsSerializer();

            var json = serializer.Serialize(serializableObject, "not valid fields");
            var serializableObjectFromJson = JsonConvert.DeserializeObject<RootSimpleTypes>(json);

            Assert.AreEqual(0, serializableObjectFromJson.Items.Count);
        }

        [Test]
        public void Should_serialize_these_fields_json_when_valid_fields_parameter_passed()
        {
            var serializableObject = new RootSimpleTypes();
            serializableObject.Items.Add(new SimpleTypes()
            {
                FirstProperty = "first property value",
                SecondProperty = "second property value",
            });

            IJsonFieldsSerializer serializer = new JsonFieldsSerializer();

            var json = serializer.Serialize(serializableObject, "first_property");
            var serializableObjectFromJson = JsonConvert.DeserializeObject<RootSimpleTypes>(json);

            Assert.AreEqual(1, serializableObjectFromJson.Items.Count);
            Assert.AreEqual("first property value", serializableObjectFromJson.Items[0].FirstProperty);

            //Should_not_serialize_other_fields_json_when_valid_fields_parameter_passed
            Assert.IsNull(serializableObjectFromJson.Items[0].SecondProperty);
        }

        #endregion

        #region With complex type test object

        [Test]
        public void Should_serialize_fields_json_complex_object_when_valid_fields_parameter_passed()
        {
            var serializableObject = new RootComplexTypes();
            serializableObject.Items.Add(new ComplexTypes
            {
                StringProperty = "string value",
                SimpleTypes = new SimpleTypes
                {
                    FirstProperty = "first property value",
                    SecondProperty = "second property value"
                },
                ListOfSimpleTypes = new List<SimpleTypes>()
                {
                    new SimpleTypes()
                    {
                        FirstProperty = "first property of list value",
                        SecondProperty = "second property of list value"
                    }
                }
            });

            IJsonFieldsSerializer serializer = new JsonFieldsSerializer();

            var json = serializer.Serialize(serializableObject, "string_property, simple_types, list_of_simple_types");
            var serializableObjectFromJson = JsonConvert.DeserializeObject<RootComplexTypes>(json);

            Assert.AreEqual(1, serializableObjectFromJson.Items.Count);
            Assert.AreEqual(1, serializableObjectFromJson.Items[0].ListOfSimpleTypes.Count);
            Assert.AreEqual("string value", serializableObjectFromJson.Items[0].StringProperty);
        }

        [Test]
        public void Should_serialize_empty_json_complex_object_when_second_level_valid_fields_passed()
        {
            var serializableObject = new RootComplexTypes();
            serializableObject.Items.Add(new ComplexTypes
            {
                StringProperty = "string value",
                SimpleTypes = new SimpleTypes
                {
                    FirstProperty = "first property value",
                    SecondProperty = "second property value"
                },
                ListOfSimpleTypes = new List<SimpleTypes>
                {
                    new SimpleTypes
                    {
                        FirstProperty = "first property of list value",
                        SecondProperty = "second property of list value"
                    }
                }
            });

            IJsonFieldsSerializer serializer = new JsonFieldsSerializer();

            var json = serializer.Serialize(serializableObject, "first_property");
            var serializableObjectFromJson = JsonConvert.DeserializeObject<RootComplexTypes>(json);

            Assert.AreEqual(0, serializableObjectFromJson.Items.Count);
        }

        [Test]
        public void Should_serialize_fields_json_complex_object_empty_list_when_valid_fields_parameter_passed()
        {
            var serializableObject = new RootComplexTypes();
            serializableObject.Items.Add(new ComplexTypes
            {
                ListOfSimpleTypes = new List<SimpleTypes>()
            });

            IJsonFieldsSerializer serializer = new JsonFieldsSerializer();

            var json = serializer.Serialize(serializableObject, "list_of_simple_types");
            var serializableObjectFromJson = JsonConvert.DeserializeObject<RootComplexTypes>(json);

            Assert.AreEqual(1, serializableObjectFromJson.Items.Count);
            Assert.AreEqual(0, serializableObjectFromJson.Items[0].ListOfSimpleTypes.Count);
        }

        [Test]
        public void Should_serialize_fields_json_complex_object_when_invalid_fields_parameter_passed()
        {
            var serializableObject = new RootComplexTypes();
            serializableObject.Items.Add(new ComplexTypes
            {
                StringProperty = "string value",
                SimpleTypes = new SimpleTypes
                {
                    FirstProperty = "first property value",
                    SecondProperty = "second property value"
                },
                ListOfSimpleTypes = new List<SimpleTypes>
                {
                    new SimpleTypes
                    {
                        FirstProperty = "first property of list value",
                        SecondProperty = "second property of list value"
                    }
                }
            });

            IJsonFieldsSerializer serializer = new JsonFieldsSerializer();

            var json = serializer.Serialize(serializableObject, "invalid field");
            var serializableObjectFromJson = JsonConvert.DeserializeObject<RootComplexTypes>(json);

            Assert.AreEqual(0, serializableObjectFromJson.Items.Count);
        }

        #endregion

        #region Private methods

        private void WhenNullObjectToSerializedPassed()
        {
            IJsonFieldsSerializer serializer = new JsonFieldsSerializer();

            serializer.Serialize(It.IsAny<ISerializableObject>(), It.IsAny<string>());
        }

        #endregion
    }
}