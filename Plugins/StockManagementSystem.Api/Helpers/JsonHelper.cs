using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace StockManagementSystem.Api.Helpers
{
    public class JsonHelper : IJsonHelper
    {
        public Dictionary<string, object> GetRequestJsonDictionaryFromStream(Stream stream, bool rewindStream)
        {
            var json = GetRequestBodyString(stream, rewindStream);
            if (string.IsNullOrEmpty(json))
                throw new InvalidOperationException("No Json provided");

            var requestBodyDictionary = DeserializeToDictionary(json);
            if (requestBodyDictionary == null || requestBodyDictionary.Count == 0)
                throw new InvalidOperationException("Json format is invalid");

            return requestBodyDictionary;
        }

        public string GetRootPropertyName<T>() where T : class, new()
        {
            var rootProperty = "";

            var jsonObjectAttribute = ReflectionHelper.GetJsonObjectAttribute(typeof(T));
            if (jsonObjectAttribute != null)
            {
                rootProperty = jsonObjectAttribute.Title;
            }

            if (string.IsNullOrEmpty(rootProperty))
                throw new InvalidOperationException($"Error getting root property for type {typeof(T).FullName}.");

            return rootProperty;
        }

        private Dictionary<string, object> DeserializeToDictionary(string json)
        {
            try
            {
                return ToObject(JToken.Parse(json)) as Dictionary<string, object>;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private object ToObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.Children<JProperty>().ToDictionary(prop => prop.Name, prop => ToObject(prop.Value));

                case JTokenType.Array:
                    return token.Select(ToObject).ToList();

                default:
                    return ((JValue)token).Value;
            }
        }

        private string GetRequestBodyString(Stream stream, bool rewindStream)
        {
            var result = "";

            using (var streamReader = new StreamReader(stream, Encoding.UTF8, true, 1024, rewindStream))
            {
                result = streamReader.ReadToEnd();
                if (rewindStream)
                {
                    stream.Position = 0; //reset position to allow reading again later
                }
            }

            return result;
        }
    }
}