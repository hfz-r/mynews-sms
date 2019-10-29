using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Configuration;

namespace StockManagementSystem.Services.Helpers
{
    public partial class DateTimeHelper : IDateTimeHelper
    {
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;

        public DateTimeHelper(
            DateTimeSettings dateTimeSettings,
            IGenericAttributeService genericAttributeService, 
            ISettingService settingService, 
            IWorkContext workContext)
        {
            _dateTimeSettings = dateTimeSettings;
            _genericAttributeService = genericAttributeService;
            _settingService = settingService;
            _workContext = workContext;
        }

        /// <summary>
        /// Retrieves a System.TimeZoneInfo object from the registry based on its identifier.
        /// </summary>
        public virtual TimeZoneInfo FindTimeZoneById(string id)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(id);
        }

        /// <summary>
        /// Returns a sorted collection of all the time zones
        /// </summary>
        public virtual ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones();
        }

        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        public virtual DateTime ConvertToUserTime(DateTime dt)
        {
            return ConvertToUserTime(dt, dt.Kind);
        }

        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        public virtual DateTime ConvertToUserTime(DateTime dt, DateTimeKind sourceDateTimeKind)
        {
            dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
            if (sourceDateTimeKind == DateTimeKind.Local && TimeZoneInfo.Local.IsInvalidTime(dt))
                return dt;

            var currentUserTimeZoneInfo = CurrentTimeZone;
            return TimeZoneInfo.ConvertTime(dt, currentUserTimeZoneInfo);
        }

        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        public virtual DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone)
        {
            var currentUserTimeZoneInfo = CurrentTimeZone;
            return ConvertToUserTime(dt, sourceTimeZone, currentUserTimeZoneInfo);
        }

        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        public virtual DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
        {
            if (sourceTimeZone.IsInvalidTime(dt))
                return dt;

            return TimeZoneInfo.ConvertTime(dt, sourceTimeZone, destinationTimeZone);
        }

        /// <summary>
        /// Converts the date and time to Coordinated Universal Time (UTC)
        /// </summary>
        public virtual DateTime ConvertToUtcTime(DateTime dt)
        {
            return ConvertToUtcTime(dt, dt.Kind);
        }

        /// <summary>
        /// Converts the date and time to Coordinated Universal Time (UTC)
        /// </summary>
        public virtual DateTime ConvertToUtcTime(DateTime dt, DateTimeKind sourceDateTimeKind)
        {
            dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
            if (sourceDateTimeKind == DateTimeKind.Local && TimeZoneInfo.Local.IsInvalidTime(dt))
                return dt;

            return TimeZoneInfo.ConvertTimeToUtc(dt);
        }

        /// <summary>
        /// Converts the date and time to Coordinated Universal Time (UTC)
        /// </summary>
        public virtual DateTime ConvertToUtcTime(DateTime dt, TimeZoneInfo sourceTimeZone)
        {
            if (sourceTimeZone.IsInvalidTime(dt))
            {
                //could not convert
                return dt;
            }

            return TimeZoneInfo.ConvertTimeToUtc(dt, sourceTimeZone);
        }

        /// <summary>
        /// Gets a user time zone
        /// </summary>
        public async Task<TimeZoneInfo> GetUserTimeZone(User user)
        {
            if (!_dateTimeSettings.AllowUsersToSetTimeZone)
                return DefaultStoreTimeZone;

            TimeZoneInfo timeZoneInfo = null;

            var timeZoneId = string.Empty;
            if (user != null)
                timeZoneId = await _genericAttributeService.GetAttributeAsync<string>(user, UserDefaults.TimeZoneIdAttribute);

            try
            {
                if (!string.IsNullOrEmpty(timeZoneId))
                    timeZoneInfo = FindTimeZoneById(timeZoneId);
            }
            catch (Exception exc)
            {
                Debug.Write(exc.ToString());
            }

            return timeZoneInfo ?? DefaultStoreTimeZone;
        }

        /// <summary>
        /// Gets or sets a default store time zone
        /// </summary>
        public virtual TimeZoneInfo DefaultStoreTimeZone
        {
            get
            {
                TimeZoneInfo timeZoneInfo = null;
                try
                {
                    if (!string.IsNullOrEmpty(_dateTimeSettings.DefaultTimeZoneId))
                        timeZoneInfo = FindTimeZoneById(_dateTimeSettings.DefaultTimeZoneId);
                }
                catch (Exception exc)
                {
                    Debug.Write(exc.ToString());
                }

                return timeZoneInfo ?? TimeZoneInfo.Local;
            }
            set
            {
                var defaultTimeZoneId = string.Empty;
                if (value != null)
                    defaultTimeZoneId = value.Id;

                _dateTimeSettings.DefaultTimeZoneId = defaultTimeZoneId;
                _settingService.SaveSetting(_dateTimeSettings);
            }
        }

        /// <summary>
        /// Gets or sets the current user time zone
        /// </summary>
        public virtual TimeZoneInfo CurrentTimeZone
        {
            get => Task.Run(async () => await GetUserTimeZone(_workContext.CurrentUser)).Result;
            set
            {
                if (!_dateTimeSettings.AllowUsersToSetTimeZone)
                    return;

                var timeZoneId = string.Empty;
                if (value != null)
                {
                    timeZoneId = value.Id;
                }

                _genericAttributeService.SaveAttributeAsync(_workContext.CurrentUser, 
                    UserDefaults.TimeZoneIdAttribute, timeZoneId);
            }
        }
    }
}