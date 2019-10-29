using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Services.Security;

namespace StockManagementSystem.Web.Mvc.Filters
{
    /// <summary>
    /// Represents a filter attribute that confirms access
    /// </summary>
    public class AuthorizeUserAttribute : TypeFilterAttribute
    {
        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public AuthorizeUserAttribute(bool ignore = false) : base(typeof(AuthorizeUserFilter))
        {
            IgnoreFilter = ignore;
            Arguments = new object[] { ignore };
        }

        public bool IgnoreFilter { get; }

        private class AuthorizeUserFilter : IAsyncAuthorizationFilter
        {
            private readonly bool _ignoreFilter;
            private readonly IPermissionService _permissionService;

            public AuthorizeUserFilter(bool ignoreFilter, IPermissionService permissionService)
            {
                _ignoreFilter = ignoreFilter;
                _permissionService = permissionService;
            }

            public async Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
            {
                if (filterContext == null)
                    throw new ArgumentNullException(nameof(filterContext));

                //check whether this filter has been overridden for the action
                var actionFilter = filterContext.ActionDescriptor.FilterDescriptors
                    .Where(fd => fd.Scope == FilterScope.Action)
                    .Select(fd => fd.Filter)
                    .OfType<AuthorizeUserAttribute>().FirstOrDefault();

                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                if (filterContext.Filters.Any(filter => filter is AuthorizeUserFilter))
                {
                    if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessPanel))
                        filterContext.Result = new ChallengeResult();
                }
            }
        }
    }
}