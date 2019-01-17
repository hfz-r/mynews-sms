using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using StockManagementSystem.Core.Domain.Identity;

namespace StockManagementSystem.Services.Helpers
{
    public interface IDateTimeHelper
    {
        TimeZoneInfo CurrentTimeZone { get; set; }

        DateTime ConvertToUserTime(DateTime dt);
        DateTime ConvertToUserTime(DateTime dt, DateTimeKind sourceDateTimeKind);
        DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone);
        DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone);
        DateTime ConvertToUtcTime(DateTime dt);
        DateTime ConvertToUtcTime(DateTime dt, DateTimeKind sourceDateTimeKind);
        DateTime ConvertToUtcTime(DateTime dt, TimeZoneInfo sourceTimeZone);
        TimeZoneInfo FindTimeZoneById(string id);
        Task<TimeZoneInfo> GetUserTimeZone(User user);
        ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones();
    }
}