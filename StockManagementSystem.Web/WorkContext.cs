using System;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Services.Authentication;

namespace StockManagementSystem.Web
{
    /// <summary>
    /// Represents work context for web application
    /// </summary>
    public partial class WorkContext : IWorkContext
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private User _cachedUser;

        public WorkContext(
            IAuthenticationService authenticationService, 
            IHttpContextAccessor httpContextAccessor)
        {
            _authenticationService = authenticationService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get user cookie
        /// </summary>
        protected virtual string GetUserCookie()
        {
            var cookieName = ".sms.user";
            return _httpContextAccessor.HttpContext?.Request?.Cookies[cookieName];
        }

        /// <summary>
        /// Set user cookie
        /// </summary>
        protected virtual void SetUserCookie(Guid userGuid)
        {
            if (_httpContextAccessor.HttpContext?.Response == null)
                return;

            //delete current cookie value
            var cookieName = ".sms.user";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

            //get date of cookie expiration
            var cookieExpires = 24 * 365; //TODO make configurable
            var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

            if (userGuid == Guid.Empty)
                cookieExpiresDate = DateTime.Now.AddMonths(-1);

            //set new cookie value
            var options = new CookieOptions
            {
                HttpOnly = true,
                Expires = cookieExpiresDate
            };
            
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, userGuid.ToString(), options);
        }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public virtual User CurrentUser
        {
            get
            {
                //whether there is a cached value
                if (_cachedUser != null)
                    return _cachedUser;

                var user = _authenticationService.GetAuthenticatedUserAsync().GetAwaiter().GetResult();
                if (user != null)
                {
                    SetUserCookie(user.UserGuid);
                    _cachedUser = user;
                }

                return _cachedUser;
            }
            set
            {
                SetUserCookie(value.UserGuid);
                _cachedUser = value;
            }
        }
    }
}