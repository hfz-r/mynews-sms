using System;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Http;
using StockManagementSystem.Services.Authentication;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Web
{
    /// <summary>
    /// Represents work context for web application
    /// </summary>
    public partial class WorkContext : IWorkContext
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private User _cachedUser;

        public WorkContext(IUserService userService, IAuthenticationService authenticationService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _authenticationService = authenticationService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get user cookie
        /// </summary>
        protected virtual string GetUserCookie()
        {
            var cookieName = $"{CookieDefaults.Prefix}{CookieDefaults.UserCookie}";
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
            var cookieName = $"{CookieDefaults.Prefix}{CookieDefaults.UserCookie}";
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

                User user = null;

                //check whether request is made by a background (schedule) task
                if (_httpContextAccessor.HttpContext == null)
                {
                    //in this case return built-in user record for background task
                    user = _userService.GetUserBySystemName(UserDefaults.BackgroundTaskUserName);
                }

                if (user == null || user.Deleted || !user.Active)
                {
                    //try to get registered user
                    user = _authenticationService.GetAuthenticatedUserAsync().GetAwaiter().GetResult();
                }

                if (user == null || user.Deleted || !user.Active)
                {
                    //get guest user
                    var userCookie = GetUserCookie();
                    if (!string.IsNullOrEmpty(userCookie))
                    {
                        if (Guid.TryParse(userCookie, out Guid userGuid))
                        {
                            //get user from cookie (should not be registered)
                            var userByCookie = _userService.GetUserByGuid(userGuid);
                            if (userByCookie != null && !userByCookie.IsRegistered())
                                user = userByCookie;
                        }
                    }
                }

                if (user == null || user.Deleted || !user.Active)
                {
                    //create guest if not exists
                    user = _userService.InsertGuestUser();
                }

                if (!user.Deleted && user.Active)
                {
                    //set user cookie
                    SetUserCookie(user.UserGuid);

                    //cache the found user
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