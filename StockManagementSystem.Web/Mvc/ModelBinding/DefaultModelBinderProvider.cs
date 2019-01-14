using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Web.Mvc.ModelBinding
{
    public class DefaultModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var modelType = context.Metadata.ModelType;
            if (!typeof(BaseModel).IsAssignableFrom(modelType))
                return null;

            //use DefaultModelBinder as a ComplexTypeModelBinder for BaseModel
            if (context.Metadata.IsComplexType && !context.Metadata.IsCollectionType)
            {
                //create binders for all model properties
                var propertyBinders = context.Metadata.Properties
                    .ToDictionary(modelProperty => modelProperty, modelProperty => context.CreateBinder(modelProperty));

                return new DefaultModelBinder(propertyBinders, EngineContext.Current.Resolve<ILoggerFactory>());
            }

            return null;
        }
    }
}