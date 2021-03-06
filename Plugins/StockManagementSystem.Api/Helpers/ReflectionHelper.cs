﻿using System;
using System.Reflection;
using Newtonsoft.Json;
using StockManagementSystem.Api.Constants;

namespace StockManagementSystem.Api.Helpers
{
    public static class ReflectionHelper
    {
        public static bool HasProperty(string propertyName, Type type)
        {
            return type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
        }

        public static PropertyInfo GetPropertyInfo(ref string propertyName, Type type)
        {
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                propertyName = Configurations.NonCrudTableSuffix + propertyName;

                propertyInfo = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            }

            return propertyInfo;
        }

        public static JsonObjectAttribute GetJsonObjectAttribute(Type objectType)
        {
            var jsonObject = objectType.GetCustomAttribute(typeof(JsonObjectAttribute)) as JsonObjectAttribute;

            return jsonObject;
        }

        public static bool HasDefaultConstructor(this Type t)
        {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }

        public static Type GetGenericElementType(Type type)
            => type.HasElementType ? type.GetElementType() : type.GetTypeInfo().GenericTypeArguments[0];
    }
}