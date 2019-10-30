using System;

namespace StockManagementSystem.Api.Json.Extensions
{
    public static class JsonPropertyExtension
    {
        public static string GetNormalizedPropertyName(this string normalizedName)
        {
            return normalizedName.Substring(normalizedName.Length - 1).Equals("s", StringComparison.CurrentCultureIgnoreCase)
                ? normalizedName.ToLowerInvariant() : string.Concat(normalizedName, "s").ToLowerInvariant();
        }
    }
}