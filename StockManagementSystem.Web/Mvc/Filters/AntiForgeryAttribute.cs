using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Logging;
using StockManagementSystem.Core.Domain.Security;

namespace StockManagementSystem.Web.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute that enables anti-forgery feature
    /// </summary>
    public class AntiForgeryAttribute : TypeFilterAttribute
    {
        private readonly bool _ignoreFilter;

        public AntiForgeryAttribute(bool ignore = false) : base(typeof(AntiForgeryFilter))
        {
            _ignoreFilter = ignore;
            Arguments = new object[] {ignore};
        }

        public bool IgnoreFilter => _ignoreFilter;

        private class AntiForgeryFilter : ValidateAntiforgeryTokenAuthorizationFilter
        {
            private readonly bool _ignoreFilter;
            private readonly SecuritySettings _securitySettings;

            public AntiForgeryFilter(
                bool ignoreFilter,
                SecuritySettings securitySettings,
                IAntiforgery antiforgery, 
                ILoggerFactory loggerFactory) : base(antiforgery, loggerFactory)
            {
                _ignoreFilter = ignoreFilter;
                _securitySettings = securitySettings;
            }

            protected override bool ShouldValidate(AuthorizationFilterContext context)
            {
                if (!base.ShouldValidate(context))
                    return false;

                if (context.HttpContext.Request == null)
                    return false;

                //ignore GET requests
                if (context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get,
                    StringComparison.InvariantCultureIgnoreCase))
                    return false;

                if (!_securitySettings.EnableXsrfProtection)
                    return false;

                var actionFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(descriptor => descriptor.Scope == FilterScope.Action)
                    .Select(descriptor => descriptor.Filter).OfType<AntiForgeryAttribute>()
                    .FirstOrDefault();

                //ignore this filter
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return false;

                return true;
            }
        }
    }
}