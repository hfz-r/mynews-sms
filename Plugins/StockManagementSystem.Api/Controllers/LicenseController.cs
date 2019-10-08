using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.DTOs.Errors;
using StockManagementSystem.Api.DTOs.License;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.Controllers
{
    public class LicenseController : BaseApiController
    {
        private readonly ILicenseApiService _licenseApiService;

        public LicenseController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            IUserService userService,
            ITenantMappingService tenantMappingService,
            ITenantService tenantService,
            IUserActivityService userActivityService,
            ILicenseApiService licenseApiService)
            : base(jsonFieldsSerializer, aclService, userService, tenantMappingService, tenantService, userActivityService)
        {
            _licenseApiService = licenseApiService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/api/license/validate")]
        [ProducesResponseType(typeof(LicenseRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Validate([FromBody] string rawString)
        {
            if (string.IsNullOrEmpty(rawString))
                return await Error(HttpStatusCode.BadRequest, "payload", "invalid payload");

            var validHeader = Request.Headers.TryGetValue("Device-SerialNo", out var val);
            if (!validHeader)
                return await Error(HttpStatusCode.BadRequest, "serial-no", "invalid header");

            var result = await _licenseApiService.ValidateLicense(rawString, val.FirstOrDefault());
            if (result != null && result.Length > 0)
                return await Error(HttpStatusCode.Unauthorized, "license", string.Join(" ", result));

            await UserActivityService.InsertActivityAsync("ValidateLicense", $"Validated license on device (ID = {val.FirstOrDefault()})");

            return Ok(new LicenseRootObject {IsValid = true });
        }
    }
}