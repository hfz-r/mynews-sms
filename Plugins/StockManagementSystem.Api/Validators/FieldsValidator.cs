using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StockManagementSystem.Api.Validators
{
    public class FieldsValidator : IFieldsValidator
    {
        private static IEnumerable<string> GetPropertiesIntoList(string fields)
        {
            var properties = fields.ToLowerInvariant()
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            return properties;
        }

        public Dictionary<string, bool> GetValidFields(string fields, Type type)
        {
            // Ensures fields won't be null
            // Check underscore on words - by specification (each word separated from others with underscore)
            fields = fields ?? string.Empty;
            fields = fields.Replace("_", string.Empty);

            var fieldsAsList = GetPropertiesIntoList(fields);

            return (from field in fieldsAsList
                let propertyExists =
                    type.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null
                where propertyExists
                select field).ToDictionary(field => field, field => true);
        }
    }
}