using System;
using System.Text.RegularExpressions;

namespace StockManagementSystem.Api.Helpers
{
    public static class GenericPropertyHelper
    {
        public static string GetGenericProperty(string typeName)
        {
            string normalizedName = String.Empty;

            string[] patternList = { "Dto", "MasterDto" };
            string pattern = $"({string.Join("|", patternList)})";

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            var match = regex.Match(typeName);
            if (match.Success)
                normalizedName = typeName.Substring(0, typeName.IndexOf(match.Value, StringComparison.InvariantCultureIgnoreCase));

            return normalizedName;
        }
    }
}