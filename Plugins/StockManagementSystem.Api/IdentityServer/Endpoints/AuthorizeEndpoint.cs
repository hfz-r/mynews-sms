using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using IdentityServer4.Endpoints.Results;
using IdentityServer4.Hosting;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;

namespace StockManagementSystem.Api.IdentityServer.Endpoints
{
    public class AuthorizeEndpoint : AuthorizeEndpointBase
    {
        public AuthorizeEndpoint(
            IEventService events,
            IAuthorizeRequestValidator validator,
            IAuthorizeInteractionResponseGenerator interactionGenerator,
            IAuthorizeResponseGenerator authorizeResponseGenerator,
            IUserSession userSession)
            : base(events, validator, authorizeResponseGenerator, interactionGenerator, userSession)
        {
        }

        public override async Task<IEndpointResult> ProcessAsync(HttpContext context)
        {
            NameValueCollection values;

            switch (context.Request.Method)
            {
                case "GET":
                    values = context.Request.Query.AsNameValueCollection();
                    break;
                case "POST":
                    if (!context.Request.HasFormContentType)
                    {
                        return new StatusCodeResult(HttpStatusCode.UnsupportedMediaType);
                    }

                    values = context.Request.Form.AsNameValueCollection();
                    break;
                default:
                    return new StatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }

            var user = await UserSession.GetUserAsync();
            var result = await ProcessAuthorizeRequestAsync(values, user, null);

            return result;
        }
    }
}