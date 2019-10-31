using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Api.Json.Extensions;

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
                if (type != null && !type.IsGenericType)
                    throw new InvalidOperationException($"{type} is not a generic type");
                if (type != null && type.IsGenericTypeDefinition)
                    throw new InvalidOperationException(
                        $"{type} is a generic type definition, it must be a constructed generic type");

                var typeArgs = type?.GetGenericArguments();
                if (attr.TypeParameterPosition >= typeArgs?.Length)
                    throw new ArgumentException(
                        $"Can't get type argument at position {attr.TypeParameterPosition}; {type} has only {typeArgs.Length} type arguments");

                var typeName = typeArgs?[attr.TypeParameterPosition].Name;
                var normalizedName = GenericPropertyHelper.GetGenericProperty(typeName);

                prop.PropertyName = !string.IsNullOrEmpty(normalizedName)
                    //? normalizedName.GetNormalizedPropertyName()
                    ? string.Concat(normalizedName, "s").ToLowerInvariant()
                    : throw new InvalidOperationException($"{typeName} is not a valid context.");
            }

            return prop;
        }
    }
}