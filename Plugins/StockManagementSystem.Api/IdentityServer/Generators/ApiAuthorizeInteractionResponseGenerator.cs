using System;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace StockManagementSystem.Api.IdentityServer.Generators
{
    public class ApiAuthorizeInteractionResponseGenerator : IAuthorizeInteractionResponseGenerator
    {
        protected readonly ILogger Logger;
        protected readonly IConsentService Consent;
        protected readonly IProfileService Profile;
        protected readonly ISystemClock Clock;

        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeInteractionResponseGenerator"/> class.
        /// </summary>
        public ApiAuthorizeInteractionResponseGenerator(
            ISystemClock clock,
            ILogger<AuthorizeInteractionResponseGenerator> logger,
            IConsentService consent,
            IProfileService profile,
            IHttpContextAccessor httpContextAccessor)
        {
            Clock = clock;
            Logger = logger;
            Consent = consent;
            Profile = profile;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Processes the interaction logic.
        /// </summary>
        public virtual async Task<InteractionResponse> ProcessInteractionAsync(ValidatedAuthorizeRequest request,
            ConsentResponse consent = null)
        {
            Logger.LogTrace("ProcessInteractionAsync");

            if (consent != null && consent.Granted == false && request.Subject.IsAuthenticated() == false)
            {
                // special case when anonymous user has issued a deny prior to authenticating
                Logger.LogInformation("Error: User denied consent");
                return new InteractionResponse
                {
                    Error = OidcConstants.AuthorizeErrors.AccessDenied
                };
            }

            var identityServerUser = new IdentityServerUser(request.ClientId)
            {
                DisplayName = request.Client.ClientName,
                AdditionalClaims = request.ClientClaims,
                AuthenticationTime = DateTime.UtcNow
            };

            request.Subject = identityServerUser.CreatePrincipal();

            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow
            };

            await _httpContextAccessor.HttpContext.SignInAsync(
                IdentityServerConstants.DefaultCookieAuthenticationScheme, request.Subject, authenticationProperties);

            var result = new InteractionResponse();

            return result;
        }
    }
}