using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Api.Attributes;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Delta;
using StockManagementSystem.Api.DTOs.Errors;
using StockManagementSystem.Api.DTOs.Roles;
using StockManagementSystem.Api.Factories;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Json.ActionResults;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Api.ModelBinders;
using StockManagementSystem.Api.Models.RolesParameters;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.Controllers
{
    public class RolesController : BaseApiController
    {
        private readonly IRoleApiService _roleApiService;
        private readonly IPermissionService _permissionService;
        private readonly IFactory<Role> _factory;

        public RolesController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            IUserService userService,
            ITenantMappingService tenantMappingService,
            ITenantService tenantService,
            IUserActivityService userActivityService,
            IRoleApiService roleApiService,
            IPermissionService permissionService,
            IFactory<Role> factory)
            : base(jsonFieldsSerializer, aclService, userService, tenantMappingService, tenantService, userActivityService)
        {
            _roleApiService = roleApiService;
            _permissionService = permissionService;
            _factory = factory;
        }

        /// <summary>
        /// Retrieve all roles
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/roles")]
        [ProducesResponseType(typeof(RolesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetRoles(RolesParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
                return await Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");

            if (parameters.Page < Configurations.DefaultPageValue)
                return await Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");

            IList<RoleDto> rolesDto =
                _roleApiService.GetRoles(
                        parameters.Ids,
                        parameters.Limit,
                        parameters.Page,
                        parameters.SinceId,
                        parameters.PermissionIds)
                    .Select(role => role.ToDto()).ToList();

            var rolesRootObject = new RolesRootObject {Roles = rolesDto};

            var json = JsonFieldsSerializer.Serialize(rolesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Get a count of all roles
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/roles/count")]
        [ProducesResponseType(typeof(RolesCountRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetRolesCount(bool active = true)
        {
            var rolesCount = _roleApiService.GetRolesCount(active);

            var rolesCountRootObject = new RolesCountRootObject {Count = rolesCount};

            return await Task.FromResult<IActionResult>(Ok(rolesCountRootObject));
        }

        /// <summary>
        /// Retrieve role by id
        /// </summary>
        /// <param name="id">Id of the role</param>
        /// <param name="fields">Fields from the role you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/roles/{id}")]
        [ProducesResponseType(typeof(RolesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> GetRoleById(int id, string fields = "")
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var role = _roleApiService.GetRoleById(id);
            if (role == null)
                return await Error(HttpStatusCode.NotFound, "role", "not found");

            var rootObj = new RolesRootObject();
            rootObj.Roles.Add(role.ToDto());

            var json = JsonFieldsSerializer.Serialize(rootObj, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Create new role
        /// </summary>
        [HttpPost]
        [Route("/api/roles")]
        [ProducesResponseType(typeof(RolesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateRole([ModelBinder(typeof(JsonModelBinder<RoleDto>))] Delta<RoleDto> roleDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var newRole = _factory.Initialize();
            roleDelta.Merge(newRole);

            await UserService.InsertRoleAsync(newRole);

            //permission
            if (roleDelta.Dto.PermissionIds.Count > 0)
            {
                await AddValidPermissions(roleDelta, newRole);
                await UserService.UpdateRoleAsync(newRole);
            }

            await UserActivityService.InsertActivityAsync("AddNewRole", $"Added a new role (ID = {newRole.Id})", newRole);

            var newRoleDto = newRole.ToDto();

            var rootObj = new RolesRootObject();
            rootObj.Roles.Add(newRoleDto);

            var json = JsonFieldsSerializer.Serialize(rootObj, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Update role by id
        /// </summary>
        [HttpPut]
        [Route("/api/roles/{id}")]
        [ProducesResponseType(typeof(RolesRootObject), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> UpdateRole([ModelBinder(typeof(JsonModelBinder<RoleDto>))] Delta<RoleDto> roleDelta)
        {
            if (!ModelState.IsValid)
                return await Error();

            var currentRole = _roleApiService.GetRoleById(roleDelta.Dto.Id);
            if (currentRole == null)
                return await Error(HttpStatusCode.NotFound, "role", "not found");
            
            roleDelta.Merge(currentRole);

            //permission
            if (roleDelta.Dto.PermissionIds.Count > 0)
                await AddValidPermissions(roleDelta, currentRole);

            await UserService.UpdateRoleAsync(currentRole);

            await UserActivityService.InsertActivityAsync("EditRole", $"Edited a role (ID = {currentRole.Id})", currentRole);

            var roleDto = currentRole.ToDto();

            var rootObj = new RolesRootObject();
            rootObj.Roles.Add(roleDto);

            var json = JsonFieldsSerializer.Serialize(rootObj, string.Empty);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Delete role by id
        /// </summary>
        [HttpDelete]
        [Route("/api/roles/{id}")]
        [ProducesResponseType(typeof(void), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int) HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IActionResult> DeleteRole(int id)
        {
            if (id <= 0)
                return await Error(HttpStatusCode.BadRequest, "id", "Invalid id");

            var role = _roleApiService.GetRoleById(id);
            if (role == null)
                return await Error(HttpStatusCode.NotFound, "role", "not found");

            await UserService.DeleteRoleAsync(role);

            await UserActivityService.InsertActivityAsync("DeleteRole", $"Deleted a role (ID = {id})", role);

            return new RawJsonActionResult("{}");
        }

        #region Private methods

        private async Task AddValidPermissions(Delta<RoleDto> roleDelta, Role currentRole)
        {
            var permissions = await _permissionService.GetAllPermissions();
            foreach (var permission in permissions)
            {
                if (roleDelta.Dto.PermissionIds.Contains(permission.Id))
                {
                    if (currentRole.PermissionRoles.Count(mapping => mapping.PermissionId == permission.Id) == 0)
                        currentRole.PermissionRoles.Add(new PermissionRoles{Permission = permission});
                }
                else
                {
                    if (currentRole.PermissionRoles.Count(mapping => mapping.PermissionId == permission.Id) > 0)
                        currentRole.PermissionRoles.Remove(
                            currentRole.PermissionRoles.FirstOrDefault(mapping =>
                                mapping.PermissionId == permission.Id));
                }
            }
        }

        #endregion
    }
}