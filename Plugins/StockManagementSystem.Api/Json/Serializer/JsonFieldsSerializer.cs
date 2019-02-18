using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using StockManagementSystem.Api.DTOs;
using StockManagementSystem.Api.Helpers;

namespace StockManagementSystem.Api.Json.Serializer
{
    public class JsonFieldsSerializer : IJsonFieldsSerializer
    {
        public string Serialize(ISerializableObject objectToSerialize, string jsonFields)
        {
            if (objectToSerialize == null)
                throw new ArgumentNullException(nameof(objectToSerialize));

            IList<string> fieldsList = null;

            if (!string.IsNullOrEmpty(jsonFields))
            {
                var primaryPropertyName = objectToSerialize.GetPrimaryPropertyName();

                fieldsList = GetPropertiesIntoList(jsonFields);

                // Always add the root manually
                fieldsList.Add(primaryPropertyName);
            }

            var json = Serialize(objectToSerialize, fieldsList);

            return json;
        }

        private static string Serialize(object objectToSerialize, IList<string> jsonFields = null)
        {
            var jToken = JToken.FromObject(objectToSerialize);

            if (jsonFields != null)
            {
                jToken = jToken.RemoveEmptyChildrenAndFilterByFields(jsonFields);
            }

            var jTokenResult = jToken.ToString();

            return jTokenResult;
        }

        private static IList<string> GetPropertiesIntoList(string fields)
        {
            IList<string> properties = fields.ToLowerInvariant()
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            return properties;
        }
    }
}