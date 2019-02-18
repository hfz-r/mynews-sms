using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Api.Converters;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Plugins;
using StockManagementSystem.Services.Messages;
using Tests;

namespace Api.Tests.ConvertersTests
{
    [TestFixture]
    public class ApiTypeConverterTests
    {
        private IApiTypeConverter _apiTypeConverter;

        [SetUp]
        public void SetUp()
        {
            _apiTypeConverter = new ApiTypeConverter();
        }

        #region ToInt

        [Test]
        [TestCase("3ed")]
        [TestCase("sd4")]
        [TestCase("675435345345345345345345343456546")]
        [TestCase("-675435345345345345345345343456546")]
        [TestCase("$%%^%^$#^&&%#)__(^&")]
        [TestCase("2015-02-12")]
        [TestCase("12:45")]
        public void Invalid_int_should_return_zero(string invalidInt)
        {
            int result = _apiTypeConverter.ToInt(invalidInt);

            result.ShouldEqual(0);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Null_or_empty_string_should_return_zero(string nullOrEmpty)
        {
            int result = _apiTypeConverter.ToInt(nullOrEmpty);

            result.ShouldEqual(0);
        }

        [Test]
        [TestCase("3")]
        [TestCase("234234")]
        [TestCase("0")]
        [TestCase("-44")]
        [TestCase("000000005")]
        public void Valid_int_should_return_valid_result(string validInt)
        {
            var isvalid = int.Parse(validInt);
            int result = _apiTypeConverter.ToInt(validInt);

            result.ShouldEqual(isvalid);
        }

        #endregion

        #region ToIntNullable

        [Test]
        [TestCase("3ed")]
        [TestCase("sd4")]
        [TestCase("675435345345345345345345343456546")]
        [TestCase("-675435345345345345345345343456546")]
        [TestCase("$%%^%^$#^&&%#)__(^&")]
        [TestCase("2015-02-12")]
        [TestCase("12:45")]
        public void Invalid_int_should_return_null(string invalidInt)
        {
            int? result = _apiTypeConverter.ToIntNullable(invalidInt);

            Assert.IsNull(result);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Null_or_empty_string_should_return_null(string nullOrEmpty)
        {
            int? result = _apiTypeConverter.ToIntNullable(nullOrEmpty);

            Assert.IsNull(result);
        }

        [Test]
        [TestCase("3")]
        [TestCase("234234")]
        [TestCase("0")]
        [TestCase("-44")]
        [TestCase("000000005")]
        public void Valid_int_should_return_that_int(string validInt)
        {
            var isvalid = int.Parse(validInt);
            int? result = _apiTypeConverter.ToIntNullable(validInt);

            result.ShouldEqual(isvalid);
        }

        #endregion

        #region ToListOfInts

        [Test]
        [TestCase("a,b,c,d")]
        [TestCase(",")]
        [TestCase("invalid")]
        [TestCase("1 2 3 4 5")]
        [TestCase("&*^&^^*()_)_-1-=")]
        [TestCase("5756797879978978978978978978978978978, 234523523423423423423423423423423423423423")]
        public void Should_return_null_when_all_parts_of_list_invalid(string invalidList)
        {
            var result = _apiTypeConverter.ToListOfInts(invalidList);

            result.ShouldBeNull();
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Should_return_null_when_null_or_empty_string(string nullOrEmpty)
        {
            var result = _apiTypeConverter.ToListOfInts(nullOrEmpty);

            result.ShouldBeNull();
        }

        [Test]
        [TestCase("1,2,3")]
        [TestCase("1, 4, 7")]
        [TestCase("0,-1, 7, 9 ")]
        [TestCase("   0,1  , 7, 9   ")]
        public void Should_return_valid_list_when_valid_list_is_passed(string validList)
        {
            var expectedList = validList.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToList();

            var result = _apiTypeConverter.ToListOfInts(validList);

            CollectionAssert.AreEqual(expectedList, result);
        }

        [Test]
        [TestCase("1,2, u,3")]
        [TestCase("a, b, c, 1")]
        [TestCase("0,-1, -, 7, 9 ")]
        [TestCase("%^#^^,$,#,%,8")]
        [TestCase("0")]
        [TestCase("097")]
        [TestCase("087, 05667, sdf")]
        [TestCase("017, 345df, 05867")]
        [TestCase("67856756, 05867, 76767ergdf")]
        [TestCase("690, 678678678678678678678678678678678678676867867")]
        public void Should_contain_only_valid_item_when_some_of_items_are_valid(string mixedList)
        {
            var expectedList = new List<int>();
            var collectionSplited = mixedList.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var item in collectionSplited)
            {
                if (int.TryParse(item, out var tempInt))
                {
                    expectedList.Add(tempInt);
                }
            }

            var result = _apiTypeConverter.ToListOfInts(mixedList);

            CollectionAssert.IsNotEmpty(result);
            CollectionAssert.AreEqual(expectedList, result);
        }

        [Test]
        [TestCase("f,d, u,3")]
        [TestCase("0")]
        [TestCase("097")]
        [TestCase("67856756, 05ert867, 76767ergdf")]
        [TestCase("690, 678678678678678678678678678678678678676867867")]
        public void Should_contain_only_valid_item_even_only_one_is_valid(string mixedList)
        {
            var expectedList = new List<int>();
            var collectionSplited = mixedList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var item in collectionSplited)
            {
                if (int.TryParse(item, out var tempInt))
                {
                    expectedList.Add(tempInt);
                }
            }

            var result = _apiTypeConverter.ToListOfInts(mixedList);

            result.Count.ShouldEqual(1);
            CollectionAssert.IsNotEmpty(result);
            CollectionAssert.AreEqual(expectedList, result);
        }

        #endregion

        #region ToEnumNullable

        [Test]
        [TestCase("-100", typeof(PasswordFormat?))]
        [TestCase("345345345345345345345345345", typeof(NotificationType?))]
        [TestCase("invalid payment status", typeof(LoadPluginsMode?))]
        [TestCase("$%%#@@$%^^))_(!34sd", typeof(PasswordFormat?))]
        public void Should_return_null_when_invalid_enum_passed_given_the_enum_type(string invalidStatus, Type type)
        {
            var result = _apiTypeConverter.ToEnumNullable(invalidStatus, type);

            Assert.IsNull(result);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void Should_return_null_when_null_or_empty_string_passed(string nullOrEmpty)
        {
            var result = _apiTypeConverter.ToEnumNullable(It.IsAny<string>(), It.IsAny<Type>());

            Assert.IsNull(result);
        }

        [Test]
        [TestCase("Hashed", typeof(PasswordFormat?))]
        [TestCase("InstalledOnly", typeof(LoadPluginsMode?))]
        [TestCase("Warning", typeof(NotificationType?))]
        public void Should_parse_valid_value_to_enum_when_valid_enum_is_passed_given_the_enum_type(string validEnum, Type type)
        {
            var enumValueParsed = Enum.Parse(Nullable.GetUnderlyingType(type), validEnum, true);

            var result = _apiTypeConverter.ToEnumNullable(validEnum, type);
            result.ShouldEqual(enumValueParsed);
        }

        #endregion

        #region ToStatus

        [Test]
        [TestCase("invalid status")]
        [TestCase("publicshed")]
        [TestCase("un-published")]
        [TestCase("322345")]
        [TestCase("%^)@*%&*@_!+=")]
        [TestCase("1")]
        public void Should_return_null_when_invalid_status_passed(string invalidStatus)
        {
            bool? result = _apiTypeConverter.ToStatus(invalidStatus);

            Assert.IsNull(result);
        }

        [Test]
        public void Should_return_valid_true_or_false_when_valid_status_passed()
        {
            var published = "published";
            bool? result = _apiTypeConverter.ToStatus(published);

            Assert.IsTrue(result != null && result.Value);

            var unpublished = "Unpublished";
            result = _apiTypeConverter.ToStatus(unpublished);

            Assert.IsFalse(result != null && result.Value);
        }

        #endregion

        #region ToDateTimeNullable

        [Test]
        [TestCase("invalid date")]
        [TestCase("20.30.10")]
        [TestCase("2016-30-30")]
        [TestCase("2016/78/12")]
        [TestCase("2016/12/12")]
        [TestCase("2016,12,34")]
        [TestCase("&^%$&(^%$_+")]
        [TestCase("2016,23,07")]
        [TestCase("2016.23.07")]
        [TestCase("2016.07.23")]
        [TestCase("0")]
        public void Should_return_null_when_invalid_date_passed(string invalidDate)
        {
            DateTime? result = _apiTypeConverter.ToUtcDateTimeNullable(invalidDate);

            Assert.IsNull(result);
        }

        [Test]
        [TestCase("2016-12")]
        [TestCase("2016-12-26")]
        [TestCase("2016-12-26T06:45")]
        [TestCase("2016-12-26T06:45:49")]
        [TestCase("2016-12-26T06:45:49.05")]
        public void Should_convert_as_utc_when_valid_Iso8601_date_without_timezone_or_offset_passed(string validDate)
        {
            DateTime expectedDateTimeUtc = DateTime.Parse(validDate, null, DateTimeStyles.RoundtripKind);

            DateTime? result = _apiTypeConverter.ToUtcDateTimeNullable(validDate);

            result.ShouldEqual(expectedDateTimeUtc);
        }

        [Test]
        [TestCase("2016-12-26T06:45:49Z")]
        [TestCase("2016-12-26T07:45:49+01:00")]
        [TestCase("2016-12-26T08:45:49+02:00")]
        [TestCase("2016-12-26T04:45:49-02:00")]
        public void Should_convert_in_utc_when_valid_time_with_timezone_or_offset_passed(string validDate)
        {
            DateTime expectedDateTimeUtc = new DateTime(2016, 12, 26, 6, 45, 49, DateTimeKind.Utc);

            DateTime? result = _apiTypeConverter.ToUtcDateTimeNullable(validDate);

            result.ShouldEqual(expectedDateTimeUtc);
        }

        #endregion
    }
}