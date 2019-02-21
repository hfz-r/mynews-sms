using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Web.Security;

namespace StockManagementSystem.Web.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute that checks whether current connection is secured and properly redirect if necessary
    /// </summary>
    public class HttpsRequirementAttribute : TypeFilterAttribute
    {
        private readonly SslRequirement _sslRequirement;

        public HttpsRequirementAttribute(SslRequirement sslRequirement) : base(typeof(HttpsRequirementFilter))
        {
            _sslRequirement = sslRequirement;
            Arguments = new object[] { sslRequirement };
        }

        public SslRequirement SslRequirement => _sslRequirement;

        private class HttpsRequirementFilter : IAuthorizationFilter
        {
            private SslRequirement _sslRequirement;
            private readonly ITenantContext _tenantContext;
            private readonly IWebHelper _webHelper;
            private readonly SecuritySettings _securitySettings;

            public HttpsRequirementFilter(
                SslRequirement sslRequirement,
                ITenantContext tenantContext,
                IWebHelper webHelper, 
                SecuritySettings securitySettings)
            {
                _sslRequirement = sslRequirement;
                _tenantContext = tenantContext;
               _webHelper = webHelper;
               _securitySettings = securitySettings;
            }

            /// <summary>
            /// Check whether current connection is secured and properly redirect if necessary
            /// </summary>
            protected void RedirectRequest(AuthorizationFilterContext filterContext, bool useSsl)
            {
                var currentConnectionSecured = _webHelper.IsCurrentConnectionSecured();

                //page should be secured, so redirect (permanent) to HTTPS version of page
                if (useSsl && !currentConnectionSecured && _tenantContext.CurrentTenant.SslEnabled)
                    filterContext.Result = new RedirectResult(_webHelper.GetThisPageUrl(true, true), true);

                //page shouldn't be secured, so redirect (permanent) to HTTP version of page
                if (!useSsl && currentConnectionSecured)
                    filterContext.Result = new RedirectResult(_webHelper.GetThisPageUrl(true, false), true);
            }

            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                if (filterContext == null)
                    throw new ArgumentNullException(nameof(filterContext));

                if (filterContext.HttpContext.Request == null)
                    return;

                //only in GET requests, otherwise the browser might not propagate the verb and request body correctly
                if (!filterContext.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return;

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                var actionFilter = filterContext.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter).OfType<HttpsRequirementAttribute>().FirstOrDefault();

                var sslRequirement = actionFilter?.SslRequirement ?? _sslRequirement;

                //whether all pages will be forced to use SSL no matter of the passed value
                if (_securitySettings.ForceSslForAllPages)
                    sslRequirement = SslRequirement.Yes;

                switch (sslRequirement)
                {
                    case SslRequirement.Yes:
                        //redirect to HTTPS page
                        RedirectRequest(filterContext, true);
                        break;
                    case SslRequirement.No:
                        //redirect to HTTP page
                        RedirectRequest(filterContext, false);
                        break;
                    case SslRequirement.NoMatter:
                        //do nothing
                        break;
                    default:
                        throw new DefaultException("Not supported SslRequirement parameter");
                }
            }
        }
    }
}