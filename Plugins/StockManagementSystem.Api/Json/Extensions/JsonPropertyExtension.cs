using System;
using StockManagementSystem.Api.Constants;

namespace StockManagementSystem.Api.Json.Extensions
{
    public static class JsonPropertyExtension
    {
        public static string GetNormalizedPropertyName(this string normalizedName)
        {
            return normalizedName.Substring(normalizedName.Length - 1)
                .Equals("s", StringComparison.CurrentCultureIgnoreCase)
                ? normalizedName.ToLowerInvariant()
                : string.Concat(normalizedName, "s").ToLowerInvariant();
        }

        public static string ResolvePropertyNamingConvention(this string property)
        {
            return property.Substring(0, 2).Equals(Configurations.NonCrudTableSuffix)
                ? property
                : property.Replace("_", string.Empty);
        }
    }
}