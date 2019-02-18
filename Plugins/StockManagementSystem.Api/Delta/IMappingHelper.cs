using System;
using System.Collections.Generic;

namespace StockManagementSystem.Api.Delta
{
    public interface IMappingHelper
    {
        void SetValues(Dictionary<string, object> propertyNameValuePairs, object objectToBeUpdated,
            Type objectToBeUpdatedType, Dictionary<object, object> objectPropertyNameValuePairs,
            bool handleComplexTypeCollections = false);

        void Merge(object source, object destination);
    }
}