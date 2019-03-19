using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using StockManagementSystem.Api.DTOs.Roles;
using StockManagementSystem.Api.Helpers;

namespace StockManagementSystem.Api.Validators
{
    public class RoleDtoValidator : BaseDtoValidator<RoleDto>
    {
        public RoleDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper,
            Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper,
            requestJsonDictionary)
        {
            SetSystemNameRule();
        }

        private void SetSystemNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(r => r.SystemName, "system_name required", "system_name");
        }
    }
}