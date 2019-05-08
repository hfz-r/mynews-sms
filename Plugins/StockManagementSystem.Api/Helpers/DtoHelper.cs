using System.Linq;
using StockManagementSystem.Api.DTOs.Devices;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Services.Security;
using StockManagementSystem.Services.Tenants;

namespace StockManagementSystem.Api.Helpers
{
    public class DtoHelper : IDtoHelper
    {
        private readonly IAclService _aclService;
        private readonly ITenantMappingService _tenantMappingService;
        private readonly ITenantService _tenantService;
        private readonly IPermissionService _permissionService;

        public DtoHelper(
            IAclService aclService, 
            ITenantMappingService tenantMappingService, 
            ITenantService tenantService,
            IPermissionService permissionService)
        {
            _aclService = aclService;
            _tenantMappingService = tenantMappingService;
            _tenantService = tenantService;
            _permissionService = permissionService;
        }

        public DeviceDto PrepareDeviceDto(Device device)
        {
            var deviceDto = device.ToDto();

            deviceDto.TenantIds = _tenantMappingService.GetTenantMappings(device)
                .GetAwaiter().GetResult()
                .Select(mapping => mapping.TenantId).ToList();

            return deviceDto;
        }
    }
}