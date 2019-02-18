using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StockManagementSystem.Api.Converters;

namespace StockManagementSystem.Api.ModelBinders
{
    public class ParametersModelBinder<T> : IModelBinder where T : class, new()
    {
        private readonly IObjectConverter _objectConverter;

        public ParametersModelBinder(IObjectConverter objectConverter)
        {
            _objectConverter = objectConverter;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            if (bindingContext.HttpContext.Request.QueryString.HasValue)
            {
                var queryParameters = bindingContext.HttpContext.Request.Query.ToDictionary(pair => pair.Key,
                        pair => pair.Value.ToString());

                bindingContext.Model = _objectConverter.ToObject<T>(queryParameters);
            }
            else
            {
                bindingContext.Model = new T();
            }

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);

            return Task.CompletedTask;
        }
    }
}