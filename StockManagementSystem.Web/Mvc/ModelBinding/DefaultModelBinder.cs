using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Web.Mvc.ModelBinding
{
    public class DefaultModelBinder : ComplexTypeModelBinder
    {
        public DefaultModelBinder(IDictionary<ModelMetadata, IModelBinder> propertyBinders, ILoggerFactory loggerFactory)
            : base(propertyBinders, loggerFactory)
        {
        }

        protected override object CreateModel(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            //create base model
            var model = base.CreateModel(bindingContext);

            //add custom model binding
            if (model is BaseModel)
                (model as BaseModel).BindModel(bindingContext);

            return model;
        }

        protected override void SetProperty(ModelBindingContext bindingContext, string modelName, ModelMetadata propertyMetadata,
            ModelBindingResult result)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var valueAsString = result.Model as string;
            if (bindingContext.Model is BaseModel && !string.IsNullOrEmpty(valueAsString))
            {
                //excluding properties with [NoTrim] attribute
                var noTrim = (propertyMetadata as DefaultModelMetadata)?.Attributes?.Attributes
                    ?.OfType<NoTrimAttribute>().Any();
                if (!noTrim.HasValue || !noTrim.Value)
                    result = ModelBindingResult.Success(valueAsString.Trim());
            }

            base.SetProperty(bindingContext, modelName, propertyMetadata, result);
        }
    }
}