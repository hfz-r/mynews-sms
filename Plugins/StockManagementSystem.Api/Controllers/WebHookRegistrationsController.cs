using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.Controllers
{
    public class WebHookRegistrationsController : BaseApiController
    {
        private const string ErrorPropertyKey = "webhook";
        private const string PrivateFilterPrefix = "MS_Private_";

        private readonly IWebHookManager _manager;
        private readonly IWebHookStore _store;
        private readonly IWebHookFilterManager _filterManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WebHookRegistrationsController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            IUserService userService,
            ITenantMappingService tenantMappingService,
            ITenantService tenantService,
            IUserActivityService userActivityService,
            IWebHookService webHookService,
            IHttpContextAccessor httpContextAccessor)
            : base(jsonFieldsSerializer, aclService, userService, tenantMappingService, tenantService, userActivityService)
        {
            _manager = webHookService.GetWebHookManager();
            _store = webHookService.GetWebHookStore();
            _filterManager = webHookService.GetWebHookFilterManager();
            _httpContextAccessor = httpContextAccessor;
        }

        #region Utilities

        private string GetUserId()
        {
            var clientId = _httpContextAccessor.HttpContext.User.FindFirst("client_id")?.Value;
            var webHookUser = clientId;

            return webHookUser;
        }

        protected virtual void RemovePrivateFilters(IEnumerable<WebHook> webHooks)
        {
            if (webHooks == null)
                throw new ArgumentNullException(nameof(webHooks));

            foreach (var webHook in webHooks)
            {
                var filters = webHook.Filters
                    .Where(f => f.StartsWith(PrivateFilterPrefix, StringComparison.OrdinalIgnoreCase)).ToArray();
                foreach (var filter in filters)
                {
                    webHook.Filters.Remove(filter);
                }
            }
        }

        /// <summary>
        /// Ensure that the provided <paramref name="webHook"/> only has registered filters.
        /// </summary>
        protected virtual async Task VerifyFilters(WebHook webHook)
        {
            if (webHook == null)
                throw new ArgumentNullException(nameof(webHook));

            if (webHook.Filters.Count == 0)
            {
                webHook.Filters.Add(WildcardWebHookFilterProvider.Name);
                return;
            }

            var filters = await _filterManager.GetAllWebHookFiltersAsync();
            var normalizedFilters = new HashSet<string>();
            var invalidFilters = new List<string>();
            foreach (var filter in webHook.Filters)
            {
                if (filters.TryGetValue(filter, out var hookFilter))
                    normalizedFilters.Add(hookFilter.Name);
                else
                    invalidFilters.Add(filter);
            }

            if (invalidFilters.Count > 0)
            {
                var invalidFiltersMsg = string.Join(", ", invalidFilters);
                var link = Url.Link(WebHookNames.FiltersGetAction, null);

                throw new VerificationException(
                    $"The following filters are not valid: '{invalidFiltersMsg}'. A list of valid filters can be obtained from the path '{link}'.");
            }

            webHook.Filters.Clear();
            foreach (var filter in normalizedFilters)
                webHook.Filters.Add(filter);
        }

        /// <summary>
        /// Ensures that the provided <paramref name="webHook"/> has a reachable Web Hook URI unless
        /// the WebHook URI has a <c>NoEcho</c> query parameter.
        /// </summary>
        private async Task VerifyWebHook(WebHook webHook)
        {
            if (webHook == null)
                throw new ArgumentNullException(nameof(webHook));

            if (string.IsNullOrEmpty(webHook.Secret))
                webHook.Secret = Guid.NewGuid().ToString("N");

            try
            {
                await _manager.VerifyWebHookAsync(webHook);
            }
            catch (Exception ex)
            {
                throw new VerificationException(ex.Message);
            }
        }

        /// <summary>
        /// Creates an <see cref="IActionResult"/> based on the provided <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The result to use when creating the <see cref="IActionResult"/>.</param>
        /// <returns>An initialized <see cref="IActionResult"/>.</returns>
        private async Task<IActionResult> CreateHttpResult(StoreResult result)
        {
            switch (result)
            {
                case StoreResult.Success:
                    return Ok();

                case StoreResult.Conflict:
                    return await Error(HttpStatusCode.Conflict);

                case StoreResult.NotFound:
                    return NotFound();

                case StoreResult.OperationError:
                    return BadRequest();

                default:
                    return await Error(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        /// <summary>
        /// Gets all registered WebHooks for a given user.
        /// </summary>
        /// <returns>A collection containing the registered <see cref="WebHook"/> instances for a given user.</returns>
        [HttpGet]
        [Route("/api/webhooks/registrations")]
        [ProducesResponseType(typeof(IEnumerable<WebHook>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IEnumerable<WebHook>> GetAllWebHooks()
        {
            var userId = GetUserId();

            IEnumerable<WebHook> webHooks = await _store.GetAllWebHooksAsync(userId);
            RemovePrivateFilters(webHooks);

            return webHooks;
        }

        /// <summary>
        /// Looks up a registered WebHook with the given <paramref name="id"/> for a given user.
        /// </summary>
        /// <returns>The registered <see cref="WebHook"/> instance for a given user.</returns>
        [HttpGet]
        [Route("/api/webhooks/registrations/{id}", Name = WebHookNames.GetWebhookByIdAction)]
        [ProducesResponseType(typeof(WebHook), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetWebHookById(string id)
        {
            var userId = GetUserId();

            var webHook = await _store.LookupWebHookAsync(userId, id);
            if (webHook != null)
            {
                RemovePrivateFilters(new []{webHook});
                return Ok(webHook);
            }

            return NotFound();
        }

        /// <summary>
        /// Registers a new WebHook for a given user.
        /// </summary>
        /// <param name="webHook">The <see cref="WebHook"/> to create.</param>
        [HttpPost]
        [Route("/api/webhooks/registrations")]
        [ProducesResponseType(typeof(StoreResult), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(HttpResponseMessage), (int) HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(HttpResponseMessage), (int) HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> RegisterWebHook([FromBody] WebHook webHook)
        {
            if (!ModelState.IsValid)
                return await Error();

            if (webHook == null)
                return BadRequest();

            var userId = GetUserId();
            try
            {
                await VerifyFilters(webHook);
                await VerifyWebHook(webHook);
            }
            catch (VerificationException ex)
            {
                return BadRequest(ex.Message);
            }

            var existingWebhooks = await GetAllWebHooks();
            var existingWebhooksForTheSameUri = existingWebhooks.Where(wh => wh.WebHookUri == webHook.WebHookUri);

            foreach (var existingWebHook in existingWebhooksForTheSameUri)
            {
                webHook.Filters.ExceptWith(existingWebHook.Filters);

                if (!webHook.Filters.Any())
                    return await Error(HttpStatusCode.Conflict, ErrorPropertyKey, "Could not register WebHook because a Webhook with the same URI and Filters is already registered.");
            }

            try
            {
                if (Request == null)
                    throw new ArgumentNullException(nameof(Request));

                webHook.Id = null;

                var result = await _store.InsertWebHookAsync(userId, webHook);

                if (result == StoreResult.Success)
                    return CreatedAtRoute(WebHookNames.GetWebhookByIdAction, new { id = webHook.Id }, webHook);

                return await CreateHttpResult(result);
            }
            catch (Exception ex)
            {
                return await Error(HttpStatusCode.Conflict, ErrorPropertyKey, $"Could not register WebHook due to error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing WebHook registration.
        /// </summary>
        /// <param name="id">The WebHook ID.</param>
        /// <param name="webHook">The new <see cref="WebHook"/> to use.</param>
        [HttpPut]
        [Route("/api/webhooks/registrations/{id}")]
        [ProducesResponseType(typeof(StoreResult), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(HttpResponseMessage), (int) HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateWebHook(string id, WebHook webHook)
        {
            if (webHook == null)
                return BadRequest();

            if (!string.Equals(id, webHook.Id, StringComparison.OrdinalIgnoreCase))
                return BadRequest();

            var userId = GetUserId();
            await VerifyFilters(webHook);
            await VerifyWebHook(webHook);

            try
            {
                var result = await _store.UpdateWebHookAsync(userId, webHook);
                return await CreateHttpResult(result);
            }
            catch (Exception ex)
            {
                return await Error(HttpStatusCode.InternalServerError, ErrorPropertyKey, $"Could not update WebHook due to error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes an existing WebHook registration.
        /// </summary>
        /// <param name="id">The WebHook ID.</param>
        [HttpDelete]
        [Route("/api/webhooks/registrations/{id}")]
        [ProducesResponseType(typeof(StoreResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpResponseMessage), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteWebHook(string id)
        {
            var userId = GetUserId();

            try
            {
                var result = await _store.DeleteWebHookAsync(userId, id);
                return await CreateHttpResult(result);
            }
            catch (Exception ex)
            {
                return await Error(HttpStatusCode.InternalServerError, ErrorPropertyKey, $"Could not delete WebHook due to error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes all existing WebHook registrations.
        /// </summary>
        [HttpDelete]
        [Route("/api/webhooks/registrations")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HttpResponseMessage), (int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteAllWebHooks()
        {
            var userId = GetUserId();

            try
            {
                await _store.DeleteAllWebHooksAsync(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return await Error(HttpStatusCode.InternalServerError, ErrorPropertyKey, $"Could not delete WebHooks due to error: {ex.Message}");
            }
        }
    }
}