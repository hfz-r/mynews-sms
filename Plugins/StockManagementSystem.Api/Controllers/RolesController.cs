using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.DTOs.Errors;
using StockManagementSystem.Api.DTOs.Roles;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Json.ActionResults;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.Controllers
{
    //TODO: complete it !!
    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RolesController : BaseApiController
    {
        public RolesController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            IUserService userService,
            ITenantMappingService tenantMappingService,
            ITenantService tenantService,
            IUserActivityService userActivityService)
            : base(jsonFieldsSerializer, aclService, userService, tenantMappingService, tenantService, userActivityService)
        {
        }

        /// <summary>
        /// Retrieve all roles
        /// </summary>
        /// <param name="fields">Fields from the role you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/roles")]
        [ProducesResponseType(typeof(RolesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetRoles(string fields = "")
        {
            var roles = UserService.GetRoles();

            IList<RoleDto> rolesDto = roles.Select(role => role.ToDto()).ToList();

            var rolesRootObject = new RolesRootObject
            {
                Roles = rolesDto
            };

            var json = JsonFieldsSerializer.Serialize(rolesRootObject, fields);

            return await Task.FromResult<IActionResult>(new RawJsonActionResult(json));
        }
    }
}