using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Tenants;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Configuration;
using StockManagementSystem.Services.Helpers;
using Tests;

namespace Services.Tests.Helpers
{
    [TestFixture]
    public class DateTimeHelperTests : ServiceTest
    {
        private Mock<IWorkContext> _workContext;
        private Mock<ITenantContext> _tenantContext;
        private Mock<IGenericAttributeService> _genericAttributeService;
        private Mock<ISettingService> _settingService;
        private DateTimeSettings _dateTimeSettings;
        private IDateTimeHelper _dateTimeHelper;
        private Tenant _tenant;

        /// <summary>
        /// (GMT+02:00) Minsk
        /// </summary>
        private string _gmtPlus2MinskTimeZoneId;

        /// <summary>
        /// (GMT+03:00) Moscow, St. Petersburg, Volgograd
        /// </summary>
        private string _gmtPlus3MoscowTimeZoneId;

        /// <summary>
        /// (GMT+07:00) Krasnoyarsk
        /// </summary>
        private string _gmtPlus7KrasnoyarskTimeZoneId;

        [SetUp]
        public new void SetUp()
        {
            _genericAttributeService = new Mock<IGenericAttributeService>();
            _settingService = new Mock<ISettingService>();

            _workContext = new Mock<IWorkContext>();

            _tenant = new Tenant { Id = 1 };
            _tenantContext = new Mock<ITenantContext>();
            _tenantContext.Setup(x => x.CurrentTenant).Returns(_tenant);

            _dateTimeSettings = new DateTimeSettings
            {
                AllowUsersToSetTimeZone = false,
                DefaultTimeZoneId = string.Empty,
            };

            _dateTimeHelper = new DateTimeHelper(_dateTimeSettings, _genericAttributeService.Object,
                _settingService.Object, _workContext.Object);

            var isUnix = Environment.OSVersion.Platform == PlatformID.Unix;

            _gmtPlus2MinskTimeZoneId = isUnix ? "Europe/Minsk" : "E. Europe Standard Time";
            _gmtPlus3MoscowTimeZoneId = isUnix ? "Europe/Moscow" : "Russian Standard Time";
            _gmtPlus7KrasnoyarskTimeZoneId = isUnix ? "Asia/Krasnoyarsk" : "North Asia Standard Time";
        }

        [Test]
        public void Can_find_systemTimeZone_by_id()
        {
            var timeZones = _dateTimeHelper.FindTimeZoneById(_gmtPlus2MinskTimeZoneId);
            timeZones.ShouldNotBeNull();
            timeZones.Id.ShouldEqual(_gmtPlus2MinskTimeZoneId);
        }

        [Test]
        public void Can_get_all_systemTimeZones()
        {
            var systemTimeZones = _dateTimeHelper.GetSystemTimeZones();
            systemTimeZones.ShouldNotBeNull();
            systemTimeZones.Any().ShouldBeTrue();
        }

        [Test]
        public void Can_get_user_timeZone_with_customTimeZones_enabled()
        {
            _dateTimeSettings.AllowUsersToSetTimeZone = true;
            _dateTimeSettings.DefaultTimeZoneId = _gmtPlus2MinskTimeZoneId;

            var user = new User {Id = 10};

            _genericAttributeService.Setup(x => x.GetAttributeAsync<string>(user, UserDefaults.TimeZoneIdAttribute, 0))
                .ReturnsAsync(_gmtPlus3MoscowTimeZoneId);

            var timeZone = _dateTimeHelper.GetUserTimeZone(user).Result;
            timeZone.ShouldNotBeNull();
            timeZone.Id.ShouldEqual(_gmtPlus3MoscowTimeZoneId);
        }

        [Test]
        public void Can_get_user_timeZone_with_customTimeZones_disabled()
        {
            _dateTimeSettings.AllowUsersToSetTimeZone = false;
            _dateTimeSettings.DefaultTimeZoneId = _gmtPlus2MinskTimeZoneId;

            var user = new User { Id = 10 };

            _genericAttributeService.Setup(x => x.GetAttributesForEntityAsync(user.Id, "User"))
                .ReturnsAsync(new List<GenericAttribute>
                {
                    new GenericAttribute
                    {
                        TenantId = 0,
                        EntityId = user.Id,
                        Key = UserDefaults.TimeZoneIdAttribute,
                        KeyGroup = "User",
                        Value = _gmtPlus3MoscowTimeZoneId,
                    }
                });

            var timeZone = _dateTimeHelper.GetUserTimeZone(user).Result;
            timeZone.ShouldNotBeNull();
            timeZone.Id.ShouldEqual(_gmtPlus2MinskTimeZoneId);
        }

        [Test]
        public void Can_convert_dateTime_to_userTime()
        {
            var sourceDateTime = TimeZoneInfo.FindSystemTimeZoneById(_gmtPlus2MinskTimeZoneId);
            sourceDateTime.ShouldNotBeNull();

            var destinationDateTime = TimeZoneInfo.FindSystemTimeZoneById(_gmtPlus7KrasnoyarskTimeZoneId);
            destinationDateTime.ShouldNotBeNull();

            //summer time
            _dateTimeHelper.ConvertToUserTime(new DateTime(2010, 06, 01, 0, 0, 0), sourceDateTime, destinationDateTime)
                .ShouldEqual(new DateTime(2010, 06, 01, 5, 0, 0));

            //winter time
            _dateTimeHelper.ConvertToUserTime(new DateTime(2010, 01, 01, 0, 0, 0), sourceDateTime, destinationDateTime)
                .ShouldEqual(new DateTime(2010, 01, 01, 5, 0, 0));
        }

        [Test]
        public void Can_convert_dateTime_to_utc_dateTime()
        {
            var sourceDateTime = TimeZoneInfo.FindSystemTimeZoneById(_gmtPlus2MinskTimeZoneId);
            sourceDateTime.ShouldNotBeNull();

            //summer time
            var dateTime1 = new DateTime(2010, 06, 01, 0, 0, 0);
            var convertedDateTime1 = _dateTimeHelper.ConvertToUtcTime(dateTime1, sourceDateTime);
            convertedDateTime1.ShouldEqual(new DateTime(2010, 05, 31, 21, 0, 0));

            //winter time
            var dateTime2 = new DateTime(2010, 01, 01, 0, 0, 0);
            var convertedDateTime2 = _dateTimeHelper.ConvertToUtcTime(dateTime2, sourceDateTime);
            convertedDateTime2.ShouldEqual(new DateTime(2009, 12, 31, 22, 0, 0));
        }
    }
}