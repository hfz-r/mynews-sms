﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagementSystem.Api.DTOs.Errors;
using StockManagementSystem.Api.Json.ActionResults;
using StockManagementSystem.Api.Json.Serializer;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Security;
using StockManagementSystem.Core.Domain.Tenants;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Tenants;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Api.Controllers
{
    [Authorize(Policy = "ApiDefaultPolicy")]
    public class BaseApiController : Controller
    {
        protected readonly IJsonFieldsSerializer JsonFieldsSerializer;
        protected readonly IAclService AclService;
        protected readonly IUserService UserService;
        protected readonly ITenantMappingService TenantMappingService;
        protected readonly ITenantService TenantService;
        protected readonly IUserActivityService UserActivityService;

        public BaseApiController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            IUserService userService,
            ITenantMappingService tenantMappingService,
            ITenantService tenantService,
            IUserActivityService userActivityService)
        {
            JsonFieldsSerializer = jsonFieldsSerializer;
            AclService = aclService;
            UserService = userService;
            TenantMappingService = tenantMappingService;
            TenantService = tenantService;
            UserActivityService = userActivityService;
        }

        protected async Task<IActionResult> Error(HttpStatusCode statusCode = (HttpStatusCode) 422,
            string propertyKey = "", string errorMessage = "")
        {
            var errors = new Dictionary<string, List<string>>();

            if (!string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(propertyKey))
            {
                var errorsList = new List<string>() {errorMessage};
                errors.Add(propertyKey, errorsList);
            }

            foreach (var item in ModelState)
            {
                var errorMessages = item.Value.Errors.Select(x => x.ErrorMessage);

                var validErrorMessages = new List<string>();
                validErrorMessages.AddRange(errorMessages.Where(message => !string.IsNullOrEmpty(message)));

                if (validErrorMessages.Count > 0)
                {
                    if (errors.ContainsKey(item.Key))
                        errors[item.Key].AddRange(validErrorMessages);
                    else
                        errors.Add(item.Key, validErrorMessages.ToList());
                }
            }

            var errorsRootObject = new ErrorsRootObject {Errors = errors};

            var errorsJson = JsonFieldsSerializer.Serialize(errorsRootObject, null);

            return await Task.FromResult<IActionResult>(new ErrorActionResult(errorsJson, statusCode));
        }

        protected async Task UpdateAclRoles<TEntity>(TEntity entity, List<int> passedRoleIds)
            where TEntity : BaseEntity, IAclSupported
        {
            if (passedRoleIds == null)
                return;

            entity.SubjectToAcl = passedRoleIds.Any();

            var existingAclRecords = await AclService.GetAclRecords(entity);
            var roles = UserService.GetRoles(true);
            foreach (var role in roles)
            {
                if (passedRoleIds.Contains(role.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.RoleId == role.Id) == 0)
                        await AclService.InsertAclRecord(entity, role.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.RoleId == role.Id);
                    if (aclRecordToDelete != null)
                        await AclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        protected async Task UpdateTenantMappings<TEntity>(TEntity entity, List<int> passedTenantIds)
            where TEntity : BaseEntity, ITenantMappingSupported
        {
            if (passedTenantIds == null)
                return;

            entity.LimitedToTenants = passedTenantIds.Any();

            var existingTenantMappings = await TenantMappingService.GetTenantMappings(entity);
            var tenants = await TenantService.GetTenantsAsync();
            foreach (var tenant in tenants)
            {
                if (passedTenantIds.Contains(tenant.Id))
                {
                    //new tenant
                    if (existingTenantMappings.Count(sm => sm.TenantId == tenant.Id) == 0)
                        await TenantMappingService.InsertTenantMapping(entity, tenant.Id);
                }
                else
                {
                    //remove tenant
                    var tenantMappingToDelete = existingTenantMappings.FirstOrDefault(sm => sm.TenantId == tenant.Id);
                    if (tenantMappingToDelete != null)
                        await TenantMappingService.DeleteTenantMapping(tenantMappingToDelete);
                }
            }
        }
    }
}