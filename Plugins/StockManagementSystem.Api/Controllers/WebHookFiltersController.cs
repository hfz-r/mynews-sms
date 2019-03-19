using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.Controllers
{
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WebHookFiltersController : BaseApiController
    {
        private readonly IWebHookFilterManager _filterManager;

        public WebHookFiltersController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            IUserService userService,
            ITenantMappingService tenantMappingService,
            ITenantService tenantService,
            IUserActivityService userActivityService,
            IWebHookService webHookService)
            : base(jsonFieldsSerializer, aclService, userService, tenantMappingService, tenantService, userActivityService)
        {
            _filterManager = webHookService.GetWebHookFilterManager();
        }

        [HttpGet]
        [Route("/api/webhooks/filters")]
        [ProducesResponseType(typeof(IEnumerable<WebHookFilter>), (int)HttpStatusCode.OK)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IEnumerable<WebHookFilter>> GetWebHookFilters()
        {
            var filters = await _filterManager.GetAllWebHookFiltersAsync();

            return filters.Values;
        }
    }
}