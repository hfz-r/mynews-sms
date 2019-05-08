using System;

namespace StockManagementSystem.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonPropertyGenericTypeNameAttribute : Attribute
    {
        public int TypeParameterPosition { get; }

        public JsonPropertyGenericTypeNameAttribute(int position)
        {
            TypeParameterPosition = position;
        }
    }
}