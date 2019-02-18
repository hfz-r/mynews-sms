using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace StockManagementSystem.Api.Converters
{
    public class ApiTypeConverter : IApiTypeConverter
    {
        public DateTime? ToUtcDateTimeNullable(string value)
        {
            var formats = new string[]
            {
                "yyyy",
                "yyyy-MM",
                "yyyy-MM-dd",
                "yyyy-MM-ddTHH:mm",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:sszzz",
                "yyyy-MM-ddTHH:mm:ss.FFFFFFFK"
            };

            if (DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime result))
            {
                // only if parsed in Local time then we need to convert it to UTC
                if (result.Kind == DateTimeKind.Local)
                    return result.ToUniversalTime();

                return result;
            }

            return null;
        }

        public int ToInt(string value)
        {
            return int.TryParse(value, out var result) ? result : 0;
        }

        public int? ToIntNullable(string value)
        {
            return int.TryParse(value, out var result) ? (int?) result : null;
        }

        public IList<int> ToListOfInts(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var stringIds = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var intIds = new List<int>();

                foreach (var id in stringIds)
                {
                    if (int.TryParse(id, out var intId))
                    {
                        intIds.Add(intId);
                    }
                }

                intIds = intIds.Distinct().ToList();
                return intIds.Count > 0 ? intIds : null;
            }

            return null;
        }

        public bool? ToStatus(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Equals("published", StringComparison.InvariantCultureIgnoreCase))
                    return true;
                if (value.Equals("unpublished", StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }

            return null;
        }

        public object ToEnumNullable(string value, Type type)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var enumType = Nullable.GetUnderlyingType(type);
                var enumNames = enumType.GetEnumNames();

                if (enumNames.Any(x => x.ToLowerInvariant().Equals(value.ToLowerInvariant())))
                    return Enum.Parse(enumType, value, true);
            }

            return null;
        }
    }
}