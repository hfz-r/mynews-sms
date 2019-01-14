using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using StockManagementSystem.Core.Domain.Identity;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Services.Authentication
{
    public class CookieAuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private User _cachedUser;

        public CookieAuthenticationService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUserService userService, 
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SignInAsync(User user, bool isPersistent)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //create claims for user's username and email
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(user.UserName))
                claims.Add(new Claim(ClaimTypes.Name, user.UserName, ClaimValueTypes.String, AuthenticationDefaults.ClaimsIssuer));

            if (!string.IsNullOrEmpty(user.Email))
                claims.Add(new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.Email, AuthenticationDefaults.ClaimsIssuer));

            if (user.UserRoles.Any())
            {
                user.UserRoles.ToList().ForEach(ur =>
                {
                    claims.Add(new Claim(ClaimTypes.Role, ur.Role.Name, ClaimValueTypes.String,
                        AuthenticationDefaults.ClaimsIssuer));
                });
            }

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
            //sign in to identity service
            await _signInManager.SignInAsync(user, isPersistent);

            //cache authenticated user
            _cachedUser = user;
        }

        public async Task SignOutAsync()
        {
            //reset cached user
            _cachedUser = null;

            //and sign out from the current authentication scheme
            await _httpContextAccessor.HttpContext.SignOutAsync(AuthenticationDefaults.AuthenticationScheme);
            //sign out from identity service
            await _signInManager.SignOutAsync();
        }

        public async Task<User> GetAuthenticatedUserAsync()
        {
            if (_cachedUser != null)
                return _cachedUser;

            var authenticateUser = await _httpContextAccessor.HttpContext.AuthenticateAsync(AuthenticationDefaults.AuthenticationScheme);
            if (!authenticateUser.Succeeded)
                return null;

            User user = null;
            var usernameClaim = authenticateUser.Principal.FindFirst(claim =>
                claim.Type == ClaimTypes.Name && claim.Issuer.Equals(AuthenticationDefaults.ClaimsIssuer,
                    StringComparison.InvariantCultureIgnoreCase));
            if (usernameClaim != null)
                user = await _userService.GetUserByUsernameAsync(usernameClaim.Value);

            if (user == null || !user.IsRegistered())
                return null;

            //cache authenticated user
            _cachedUser = user;

            return _cachedUser;
        }
    }
}