using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.Validators
{
    public interface IFieldsValidator
    {
        Dictionary<string, bool> GetValidFields(string fields, Type type);
    }
}