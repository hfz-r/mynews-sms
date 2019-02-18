using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using StockManagementSystem.Api.DTOs.Errors;
using StockManagementSystem.Api.Json.ActionResults;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.Models;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Api.Attributes
{
    public class GetRequestsErrorInterceptorActionFilter : ActionFilterAttribute
    {
        private readonly IJsonFieldsSerializer _jsonFieldsSerializer;

        public GetRequestsErrorInterceptorActionFilter()
        {
            _jsonFieldsSerializer = EngineContext.Current.Resolve<IJsonFieldsSerializer>();
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null && !actionExecutedContext.ExceptionHandled)
            {
                var error = new KeyValuePair<string, List<string>>("internal_server_error",
                    new List<string> {"Please, contact the administrator."});

                actionExecutedContext.Exception = null;
                actionExecutedContext.ExceptionHandled = true;
                SetError(actionExecutedContext, error);
            }
            else if (actionExecutedContext.HttpContext.Response != null &&
                     (HttpStatusCode) actionExecutedContext.HttpContext.Response.StatusCode != HttpStatusCode.OK)
            {
                string responseBody;
                using (var streamReader = new StreamReader(actionExecutedContext.HttpContext.Response.Body))
                {
                    responseBody = streamReader.ReadToEnd();
                }

                // reset reader possition.
                actionExecutedContext.HttpContext.Response.Body.Position = 0;

                var defaultWebApiErrorsModel = JsonConvert.DeserializeObject<DefaultWebApiErrorsModel>(responseBody);
                if (!string.IsNullOrEmpty(defaultWebApiErrorsModel.Message) &&
                    !string.IsNullOrEmpty(defaultWebApiErrorsModel.MessageDetail))
                {
                    var error = new KeyValuePair<string, List<string>>("lookup_error", new List<string> {"Not found!"});
                    SetError(actionExecutedContext, error);
                }
            }

            base.OnActionExecuted(actionExecutedContext);
        }

        private void SetError(ActionExecutedContext actionExecutedContext, KeyValuePair<string, List<string>> error)
        {
            var bindingError = new Dictionary<string, List<string>> {{error.Key, error.Value}};
            var errorsRootObject = new ErrorsRootObject
            {
                Errors = bindingError
            };

            var errorJson = _jsonFieldsSerializer.Serialize(errorsRootObject, null);

            actionExecutedContext.Result = new ErrorActionResult(errorJson, HttpStatusCode.BadRequest);
        }
    }
}