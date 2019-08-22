using System;
using AutoMapper;
using StockManagementSystem.Services.Helpers;

namespace StockManagementSystem.Api.Infrastructure.Mapper
{
    public class DateTimeResolver : IValueResolver<object, object, string>
    {
        private readonly IDateTimeHelper _dateTimeHelper;

        public DateTimeResolver(IDateTimeHelper dateTimeHelper)
        {
            _dateTimeHelper = dateTimeHelper;
        }

        public string Resolve(object source, object destination, string destMember, ResolutionContext context)
        {
            var dt = source.GetType().GetProperty("CreatedOnUtc");
            if (dt != null && dt.PropertyType == typeof(DateTime))
            {
                var d = (DateTime) dt.GetValue(source);

                var localTime = _dateTimeHelper.ConvertToUserTime(d, DateTimeKind.Utc);
                return localTime.ToString("F");
            }

            return null;
        }
    }
}