﻿using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Events;
using IdentityServer4.Hosting;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;

namespace StockManagementSystem.Api.IdentityServer.Endpoints
{
    public abstract class AuthorizeEndpointBase : IEndpointHandler
    {
        private readonly IEventService _events;
        private readonly IAuthorizeRequestValidator _validator;
        private readonly IAuthorizeResponseGenerator _authorizeResponseGenerator;
        private readonly IAuthorizeInteractionResponseGenerator _interactionGenerator;

        protected IUserSession UserSession { get; private set; }

        protected AuthorizeEndpointBase(
            IEventService events,
            IAuthorizeRequestValidator validator,
            IAuthorizeResponseGenerator authorizeResponseGenerator,
            IAuthorizeInteractionResponseGenerator interactionGenerator,
            IUserSession userSession)
        {
            _events = events;
            _validator = validator;
            _authorizeResponseGenerator = authorizeResponseGenerator;
            _interactionGenerator = interactionGenerator;
            UserSession = userSession;
        }

        public abstract Task<IEndpointResult> ProcessAsync(HttpContext context);

        protected async Task<IEndpointResult> CreateErrorResultAsync(
            string logMessage,
            ValidatedAuthorizeRequest request = null,
            string error = OidcConstants.AuthorizeErrors.ServerError,
            string errorDescription = null)
        {
            await RaiseFailureEventAsync(request, error, errorDescription);

            return new AuthorizeResult(new AuthorizeResponse
            {
                Request = request,
                Error = error,
                ErrorDescription = errorDescription
            });
        }

        protected Task RaiseResponseEventAsync(AuthorizeResponse response)
        {
            return !response.IsError
                ? _events.RaiseAsync(new TokenIssuedSuccessEvent(response))
                : RaiseFailureEventAsync(response.Request, response.Error, response.ErrorDescription);
        }

        protected Task RaiseFailureEventAsync(ValidatedAuthorizeRequest request, string error, string errorDescription)
        {
            return _events.RaiseAsync(new TokenIssuedFailureEvent(request, error, errorDescription));
        }

        protected async Task<IEndpointResult> ProcessAuthorizeRequestAsync(NameValueCollection parameters,
            ClaimsPrincipal user, ConsentResponse consent)
        {
            // validate request
            var result = await _validator.ValidateAsync(parameters, user);
            if (result.IsError)
            {
                return await CreateErrorResultAsync(
                    "Request validation failed",
                    result.ValidatedRequest,
                    result.Error,
                    result.ErrorDescription);
            }

            var request = result.ValidatedRequest;

            // determine user interaction
            var interactionResult = await _interactionGenerator.ProcessInteractionAsync(request, consent);
            if (interactionResult.IsError)
            {
                return await CreateErrorResultAsync("Interaction generator error", request, interactionResult.Error);
            }
            if (interactionResult.IsLogin)
            {
                return new LoginPageResult(request);
            }
            if (interactionResult.IsConsent)
            {
                return new ConsentPageResult(request);
            }
            if (interactionResult.IsRedirect)
            {
                return new CustomRedirectResult(request, interactionResult.RedirectUrl);
            }

            var response = await _authorizeResponseGenerator.CreateResponseAsync(request);

            await RaiseResponseEventAsync(response);

            return new AuthorizeResult(response);
        }

        protected async Task<IEndpointResult> CreateErrorResultAsync(
            ValidatedAuthorizeRequest request = null,
            string error = OidcConstants.AuthorizeErrors.ServerError,
            string errorDescription = null)
        {
            await RaiseFailureEventAsync(request, error, errorDescription);

            return new AuthorizeResult(new AuthorizeResponse
            {
                Request = request,
                Error = error,
                ErrorDescription = errorDescription
            });
        }
    }
}