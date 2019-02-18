using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.Json.Maps
{
    public interface IJsonPropertyMapper
    {
        Dictionary<string, Tuple<string, Type>> GetMap(Type type);
    }
}