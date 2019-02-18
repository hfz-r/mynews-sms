using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.Converters;
using Tests;

namespace Api.Tests.ConvertersTests
{
    [TestFixture]
    public class ObjectConverterTests
    {
        private Mock<IApiTypeConverter> _apiTypeConverter;
        private ObjectConverter _objectConverter;

        [SetUp]
        public void SetUp()
        {
            _apiTypeConverter = new Mock<IApiTypeConverter>();

            _objectConverter = new ObjectConverter(_apiTypeConverter.Object);
        }

        [Test]
        public void Should_not_call_api_converter_when_collection_isnull_or_empty()
        {
            _objectConverter.ToObject<TestingObject>(null);

            _apiTypeConverter.Verify(x => x.ToInt(It.IsAny<string>()), Times.Never);
            _apiTypeConverter.Verify(x => x.ToIntNullable(It.IsAny<string>()), Times.Never);
            _apiTypeConverter.Verify(x => x.ToUtcDateTimeNullable(It.IsAny<string>()), Times.Never);
            _apiTypeConverter.Verify(x => x.ToListOfInts(It.IsAny<string>()), Times.Never);
            _apiTypeConverter.Verify(x => x.ToStatus(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Should_not_call_api_converter_when_collection_empty()
        {
            _objectConverter.ToObject<TestingObject>(new List<KeyValuePair<string, string>>());

            _apiTypeConverter.Verify(x => x.ToInt(It.IsAny<string>()), Times.Never);
            _apiTypeConverter.Verify(x => x.ToIntNullable(It.IsAny<string>()), Times.Never);
            _apiTypeConverter.Verify(x => x.ToUtcDateTimeNullable(It.IsAny<string>()), Times.Never);
            _apiTypeConverter.Verify(x => x.ToListOfInts(It.IsAny<string>()), Times.Never);
            _apiTypeConverter.Verify(x => x.ToStatus(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Should_return_specified_object_when_collection_isnull()
        {
            var objectResult = _objectConverter.ToObject<TestingObject>(null);

            Assert.IsNotNull(objectResult);
            Assert.IsInstanceOf<TestingObject>(objectResult);
        }

        [Test]
        public void Should_return_object_with_unset_properties_when_collection_isnull()
        {
            var objectResult = _objectConverter.ToObject<TestingObject>(null);

            objectResult.IntProperty.ShouldEqual(0);
            objectResult.StringProperty.ShouldBeNull();
            objectResult.DateTimeNullableProperty.ShouldBeNull();
            objectResult.BooleanNullableStatusProperty.ShouldBeNull();
        }

        [Test]
        public void Should_return_object_with_unset_properties_when_collection_empty()
        {
            var objectResultEmpty = _objectConverter.ToObject<TestingObject>(new List<KeyValuePair<string, string>>());

            objectResultEmpty.IntProperty.ShouldEqual(0);
            objectResultEmpty.StringProperty.ShouldBeNull();
            objectResultEmpty.DateTimeNullableProperty.ShouldBeNull();
            objectResultEmpty.BooleanNullableStatusProperty.ShouldBeNull();
        }

        [Test]
        [TestCase("IntProperty")]
        [TestCase("Int_Property")]
        [TestCase("int_property")]
        [TestCase("intproperty")]
        [TestCase("inTprOperTy")]
        public void Should_call_ToInt_when_collection_contains_valid_int(string intPropertyName)
        {
            int expectedInt = 5;
            _apiTypeConverter.Setup(x => x.ToInt(It.IsAny<string>())).Returns(expectedInt);
            _objectConverter = new ObjectConverter(_apiTypeConverter.Object);

            var collection = new List<KeyValuePair<string,string>>
            {
                new KeyValuePair<string, string>(intPropertyName, expectedInt.ToString())
            };

            _objectConverter.ToObject<TestingObject>(collection);

            _apiTypeConverter.Verify(x => x.ToInt(It.IsAny<string>()));
        }

        [Test]
        [TestCase("invalid int property name")]
        [TestCase("34534535345345345345345345345345345345345")]
        public void Should_not_call_ToInt_when_collection_contains_invalid_int(string invalidIntPropertyName)
        {
            var collection = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(invalidIntPropertyName, "5")
            };

            _objectConverter.ToObject<TestingObject>(collection);

            _apiTypeConverter.Verify(x => x.ToInt(It.IsAny<string>()), Times.Never);
        }

        [Test]
        [TestCase("StringProperty")]
        [TestCase("String_Property")]
        [TestCase("string_property")]
        [TestCase("stringproperty")]
        [TestCase("strInGprOperTy")]
        public void Should_set_object_string_property_value_to_collection_string_property_value(string stringPropertyName)
        {
           var collection = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(stringPropertyName, "some value")
            };

            var objectResult = _objectConverter.ToObject<TestingObject>(collection);
            objectResult.StringProperty.ShouldEqual("some value");
        }

        [Test]
        [TestCase("invalid string property name")]
        public void
            Should_return_string_property_value_set_to_default_value_when_collection_contains_invalid_string_property(
                string invalidStringPropertyName)
        {
            var collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(invalidStringPropertyName, "some value")
            };

            var objectResult = _objectConverter.ToObject<TestingObject>(collection);
            objectResult.StringProperty.ShouldBeNull();
        }

        [Test]
        [TestCase("invalid string property name")]
        [TestCase("StringProperty")]
        [TestCase("String_Property")]
        [TestCase("string_property")]
        [TestCase("stringproperty")]
        [TestCase("strInGprOperTy")]
        public void Should_not_call_api_converter_when_collection_contains_valid_invalid_string_property(
            string stringProperty)
        {
            var collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(stringProperty, "some value")
            };

            _objectConverter.ToObject<TestingObject>(collection);

            _apiTypeConverter.Verify(x => x.ToInt(It.IsAny<string>()), Times.Never);
            _apiTypeConverter.Verify(x => x.ToIntNullable(It.IsAny<string>()), Times.Never);
            _apiTypeConverter.Verify(x => x.ToUtcDateTimeNullable(It.IsAny<string>()), Times.Never);
            _apiTypeConverter.Verify(x => x.ToListOfInts(It.IsAny<string>()), Times.Never);
            _apiTypeConverter.Verify(x => x.ToStatus(It.IsAny<string>()), Times.Never);
        }

        [Test]
        [TestCase("DateTimeNullableProperty")]
        [TestCase("Date_Time_Nullable_Property")]
        [TestCase("date_time_nullable_property")]
        [TestCase("datetimenullableproperty")]
        [TestCase("dateTimeNullableProperty")]
        public void Should_call_ToDateTimeNullable_when_collection_contains_valid_datetime_property(
            string dateTimePropertyName)
        {
            _apiTypeConverter.Setup(x => x.ToUtcDateTimeNullable(It.IsAny<string>())).Returns(DateTime.Now);
            _objectConverter = new ObjectConverter(_apiTypeConverter.Object);

            var collection = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(dateTimePropertyName, "2016-12-12")
            };

            _objectConverter.ToObject<TestingObject>(collection);

            _apiTypeConverter.Verify(x => x.ToUtcDateTimeNullable(It.IsAny<string>()));
        }

        [Test]
        [TestCase("invalid date time property name")]
        public void Should_not_call_ToDateTimeNullable_when_collection_contains_invalid_datetime_property(
            string invalidDateTimeNullablePropertyName)
        {
            _apiTypeConverter.Setup(x => x.ToUtcDateTimeNullable(It.IsAny<string>()));
            _objectConverter = new ObjectConverter(_apiTypeConverter.Object);

            var collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(invalidDateTimeNullablePropertyName, "2016-12-12")
            };

            _objectConverter.ToObject<TestingObject>(collection);

            _apiTypeConverter.Verify(x => x.ToUtcDateTimeNullable(It.IsAny<string>()), Times.Never);
        }

        [Test]
        [TestCase("BooleanNullableStatusProperty")]
        [TestCase("Boolean_Nullable_Status_Property")]
        [TestCase("boolean_nullable_status_property")]
        [TestCase("booleannullablestatusproperty")]
        [TestCase("booLeanNullabLeStaTusProperty")]
        public void Should_call_ToStatus_when_collection_contain_valid_boolean_status_property(string booleanStatusPropertyName)
        {
            _apiTypeConverter.Setup(x => x.ToStatus(It.IsAny<string>())).Returns(true);
            _objectConverter = new ObjectConverter(_apiTypeConverter.Object);

            var collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(booleanStatusPropertyName, "some published value")
            };

            _objectConverter.ToObject<TestingObject>(collection);

            _apiTypeConverter.Verify(x => x.ToStatus(It.IsAny<string>()));
        }

        [Test]
        [TestCase("invalid boolean property name")]
        public void Should_not_call_ToStatus_when_collection_contain_invalid_boolean_status_property(
            string invalidStatusPropertyName)

        {
            _apiTypeConverter.Setup(x => x.ToStatus(It.IsAny<string>()));
            _objectConverter = new ObjectConverter(_apiTypeConverter.Object);

            var collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(invalidStatusPropertyName, "true")
            };

            _objectConverter.ToObject<TestingObject>(collection);

            _apiTypeConverter.Verify(x => x.ToStatus(It.IsAny<string>()), Times.Never);
        }

        [Test]
        [TestCase("PasswordFormat")]
        [TestCase("NotificationType")]
        [TestCase("password_format")]
        [TestCase("notification_type")]
        [TestCase("passwordformat")]
        [TestCase("notificationtype")]
        [TestCase("PasswordformaT")]
        [TestCase("noTificaTioNtyPE")]
        public void Should_call_ToEnumNullable_when_collection_contain_valid_nullable_enum_property(
            string enumNullableProperty)
        {
            _apiTypeConverter.Setup(x => x.ToEnumNullable(It.IsAny<string>(), It.IsAny<Type>())).Returns(null);
            _objectConverter = new ObjectConverter(_apiTypeConverter.Object);

            var collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(enumNullableProperty, "some enum value")
            };

            _objectConverter.ToObject<TestingObject>(collection);

            _apiTypeConverter.Verify(x => x.ToEnumNullable(It.IsAny<string>(), It.IsAny<Type>()));
        }

        [Test]
        [TestCase("invalid enum property name")]
        public void Should_not_call_ToEnumNullable_when_collection_contain_invalid_nullable_enum_property(
            string invalidEnumNullableProperty)
        {
            _apiTypeConverter.Setup(x => x.ToEnumNullable(It.IsAny<string>(), It.IsAny<Type>()));
            _objectConverter = new ObjectConverter(_apiTypeConverter.Object);

            var collection = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(invalidEnumNullableProperty, "some enum value")
            };

            _objectConverter.ToObject<TestingObject>(collection);

            _apiTypeConverter.Verify(x => x.ToEnumNullable(It.IsAny<string>(), It.IsAny<Type>()), Times.Never);
        }
    }
}