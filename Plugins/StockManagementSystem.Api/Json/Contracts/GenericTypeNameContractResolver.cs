using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StockManagementSystem.Api.Attributes;

namespace StockManagementSystem.Api.Json.Contracts
{
    public class GenericTypeNameContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            var attr = member.GetCustomAttribute<JsonPropertyGenericTypeNameAttribute>();
            if (attr != null)
            {
                var type = member.DeclaringType;
                if (!type.IsGenericType)
                    throw new InvalidOperationException($"{type} is not a generic type");
                if (type.IsGenericTypeDefinition)
                    throw new InvalidOperationException($"{type} is a generic type definition, it must be a constructed generic type");

                var typeArgs = type.GetGenericArguments();
                if (attr.TypeParameterPosition >= typeArgs.Length)
                    throw new ArgumentException($"Can't get type argument at position {attr.TypeParameterPosition}; {type} has only {typeArgs.Length} type arguments");

                string normalizedName = String.Empty;
                var typeName = typeArgs[attr.TypeParameterPosition].Name;

                string[] patternList = { "Dto", "MasterDto" };
                string pattern = $"({string.Join("|", patternList)})";

                Regex regex =  new Regex(pattern, RegexOptions.IgnoreCase);

                var match = regex.Match(typeName);
                if (match.Success)
                    normalizedName = typeName.Substring(0, typeName.IndexOf(match.Value, StringComparison.InvariantCultureIgnoreCase));

                prop.PropertyName = !string.IsNullOrEmpty(normalizedName)
                    ? string.Concat(normalizedName, "s").ToLowerInvariant()
                    : throw new InvalidOperationException($"{typeName} is not from'MasterDto'");
            }

            return prop;
        }
    }
}