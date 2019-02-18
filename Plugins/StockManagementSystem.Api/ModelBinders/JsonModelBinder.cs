using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.Delta;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Api.Validators;

namespace StockManagementSystem.Api.ModelBinders
{
    public class JsonModelBinder<T> : IModelBinder where T : class, new()
    {
        private readonly IJsonHelper _jsonHelper;

        public JsonModelBinder(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyValuePairs = GetPropertyValuePairs(bindingContext);
            if (propertyValuePairs == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            if (bindingContext.ModelState.IsValid)
            {
                // id parameter passed in the model binder only when you have put request.
                var routeDataId = GetRouteDataId(bindingContext.ActionContext);
                if (routeDataId != null)
                    InsertIdInTheValuePaires(propertyValuePairs, routeDataId);

                ValidateValueTypes(bindingContext, propertyValuePairs);

                Delta<T> delta = null;

                if (bindingContext.ModelState.IsValid)
                {
                    delta = new Delta<T>(propertyValuePairs);
                    ValidateModel(bindingContext, propertyValuePairs, delta.Dto);
                }

                if (bindingContext.ModelState.IsValid)
                {
                    bindingContext.Model = delta;
                    bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }

        private Dictionary<string, object> GetPropertyValuePairs(ModelBindingContext bindingContext)
        {
            Dictionary<string, object> result = null;

            if (bindingContext.ModelState.IsValid)
            {
                try
                {
                    //get the root dictionary and root property
                    result = _jsonHelper.GetRequestJsonDictionaryFromStream(bindingContext.HttpContext.Request.Body, true);
                    var rootPropertyName = _jsonHelper.GetRootPropertyName<T>();

                    result = (Dictionary<string, object>)result[rootPropertyName];
                }
                catch (Exception ex)
                {
                    bindingContext.ModelState.AddModelError("json", ex.Message);
                }
            }

            return result;
        }

        private static object GetRouteDataId(ActionContext actionContext)
        {
            object routeDataId = null;

            if (actionContext.RouteData.Values.ContainsKey("id"))
            {
                routeDataId = actionContext.RouteData.Values["id"];
            }

            return routeDataId;
        }

        private void ValidateValueTypes(ModelBindingContext bindingContext,  Dictionary<string, object> propertyValuePaires)
        {
            var errors = new Dictionary<string, string>();

            // Validate if the property value pairs passed matches the type.
            var typeValidator = new TypeValidator<T>();
            if (!typeValidator.IsValid(propertyValuePaires))
            {
                foreach (var invalidProperty in typeValidator.InvalidProperties)
                {
                    var key = $"Invalid {invalidProperty} type";
                    if (!errors.ContainsKey(key))
                        errors.Add(key, "Invalid Property Type");
                }
            }

            if (errors.Count > 0)
            {
                foreach (var error in errors)
                    bindingContext.ModelState.AddModelError(error.Key, error.Value);
            }
        }

        private void ValidateModel(ModelBindingContext bindingContext, Dictionary<string, object> propertyValuePaires, T dto)
        {
            var dtoProperties = dto.GetType().GetProperties();
            foreach (var property in dtoProperties)
            {
                var validationAttribute =
                    property.PropertyType.GetCustomAttribute(
                        typeof(BaseValidationAttribute)) as BaseValidationAttribute ??
                    property.GetCustomAttribute(typeof(BaseValidationAttribute)) as BaseValidationAttribute;

                if (validationAttribute != null)
                {
                    validationAttribute.Validate(property.GetValue(dto));
                    var errors = validationAttribute.GetErrors();

                    if (errors.Count > 0)
                    {
                        foreach (var error in errors)
                        {
                            bindingContext.ModelState.AddModelError(error.Key, error.Value);
                        }
                    }
                }
            }
        }

        private void InsertIdInTheValuePaires(Dictionary<string, object> propertyValuePaires, object requestId)
        {
            if (propertyValuePaires.ContainsKey("id"))
                propertyValuePaires["id"] = requestId;
            else
                propertyValuePaires.Add("id", requestId);
        }
    }
}