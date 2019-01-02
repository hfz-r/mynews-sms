using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StockManagementSystem.Web.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute that check existence of passed form key and return result as an action parameter 
    /// </summary>
    public class ParameterBasedOnFormNameAttribute : TypeFilterAttribute
    {
        public ParameterBasedOnFormNameAttribute(string formKeyName, string actionParameterName) : base(
            typeof(ParameterBasedOnFormNameFilter))
        {
            Arguments = new object[] {formKeyName, actionParameterName};
        }

        private class ParameterBasedOnFormNameFilter : IActionFilter
        {
            private readonly string _formKeyName;
            private readonly string _actionParameterName;

            public ParameterBasedOnFormNameFilter(string formKeyName, string actionParameterName)
            {
                this._formKeyName = formKeyName;
                this._actionParameterName = actionParameterName;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                //if form key with '_formKeyName' exists, then set specified '_actionParameterName' to true
                context.ActionArguments[_actionParameterName] = context.HttpContext.Request.Form.Keys.Any(key => key.Equals(_formKeyName));
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
            }
        }
    }
}