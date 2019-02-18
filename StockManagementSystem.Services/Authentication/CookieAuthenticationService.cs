using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Services.Authentication
{
    public class CookieAuthenticationService : IAuthenticationService
    {
        private readonly UserSettings _userSettings;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private User _cachedUser;

        public CookieAuthenticationService(
            UserSettings userSettings,
            IUserService userService, 
            IHttpContextAccessor httpContextAccessor)
        {
            _userSettings = userSettings;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SignInAsync(User user, bool isPersistent)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //create claims for user's username and email
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(user.Username))
                claims.Add(new Claim(ClaimTypes.Name, user.Username, ClaimValueTypes.String, AuthenticationDefaults.ClaimsIssuer));

            if (!string.IsNullOrEmpty(user.Email))
                claims.Add(new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.Email, AuthenticationDefaults.ClaimsIssuer));

            //create principal for the current authentication scheme
            var userIdentity = new ClaimsIdentity(claims, AuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            //set value indicating whether session is persisted and the time at which the authentication was issued
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                IssuedUtc = DateTime.UtcNow
            };

            //sign in
            await _httpContextAccessor.HttpContext.SignInAsync(AuthenticationDefaults.AuthenticationScheme, userPrincipal, authenticationProperties);

            //cache authenticated user
            _cachedUser = user;
        }

        public async Task SignOutAsync()
        {
            //reset cached user
            _cachedUser = null;

            //and sign out from the current authentication scheme
            await _httpContextAccessor.HttpContext.SignOutAsync(AuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<User> GetAuthenticatedUserAsync()
        {
            if (_cachedUser != null)
                return _cachedUser;

            var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(AuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                return null;

            User user = null;
            if (_userSettings.UsernamesEnabled)
            {
                //try to get user by username
                var usernameClaim = authenticateResult.Principal.FindFirst(claim => 
                    claim.Type == ClaimTypes.Name && claim.Issuer.Equals(AuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                if (usernameClaim != null)
                    user = await _userService.GetUserByUsernameAsync(usernameClaim.Value);
            }
            else
            {
                //try to get user by email
                var emailClaim = authenticateResult.Principal.FindFirst(claim => 
                    claim.Type == ClaimTypes.Email && claim.Issuer.Equals(AuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                if (emailClaim != null)
                    user = await _userService.GetUserByEmailAsync(emailClaim.Value);
            }

            if (user == null || !user.Active || user.Deleted || !user.IsRegistered())
                return null;

            //cache authenticated user
            _cachedUser = user;

            return _cachedUser;
        }
    }
}