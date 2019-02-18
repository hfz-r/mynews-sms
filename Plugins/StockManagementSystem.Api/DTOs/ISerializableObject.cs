using System;

namespace StockManagementSystem.Api.DTOs
{
    public interface ISerializableObject
    {
        string GetPrimaryPropertyName();

        Type GetPrimaryPropertyType();
    }
}