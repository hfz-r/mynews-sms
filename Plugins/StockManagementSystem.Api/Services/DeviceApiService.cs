using System;
using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Api.DTOs.Devices;
using StockManagementSystem.Api.Extensions;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Services.Tenants;
using static StockManagementSystem.Api.Extensions.ApiServiceExtension;

namespace StockManagementSystem.Api.Services
{
    public class DeviceApiService : IDeviceApiService
    {
        private readonly ITenantMappingService _tenantMappingService;
        private readonly IRepository<Device> _deviceRepository;

        public DeviceApiService(
            ITenantMappingService tenantMappingService,
            IRepository<Device> deviceRepository)
        {
            _tenantMappingService = tenantMappingService;
            _deviceRepository = deviceRepository;
        }

        public IList<Device> GetDevices(IList<int> ids = null, int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, string status = "")
        {
            var query = GetDevicesQuery(createdAtMin, createdAtMax, status, ids);

            if (sinceId > 0)
                query = query.Where(device => device.Id > sinceId);

            return new ApiList<Device>(query, page - 1, limit);
        }

        public Device GetDeviceById(int id)
        {
            if (id <= 0)
                return null;

            var device = _deviceRepository.Table.FirstOrDefault(d => d.Id == id);

            return device;
        }

        public Search<DeviceDto> Search(
            string queryParams = "",
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false,
            bool count = false)
        {
            var query = _deviceRepository.Table;

            var searchParams = EnsureSearchQueryIsValid(queryParams, ResolveSearchQuery);
            if (searchParams != null)
            {
                query = query.HandleSearchParams(searchParams);
            }

            query = query.GetQuery(sortColumn, @descending);

            var _ = new SearchWrapper<DeviceDto, Device>();
            return count
                ? _.ToCount(query)
                : _.ToList(query, page, limit,
                    list => list.Select(entity => entity.ToDto()).ToList() as IList<DeviceDto>);
        }

        public Device GetDeviceBySerialNo(string serialNo)
        {
            var device =
                from d in _deviceRepository.Table
                where d.SerialNo == serialNo
                select d;

            return device.FirstOrDefault();
        }

        public int GetDevicesCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, string status = "")
        {
            var query = GetDevicesQuery(createdAtMin, createdAtMax, status);

            return query.Count(d => _tenantMappingService.Authorize(d));
        }

        private IQueryable<Device> GetDevicesQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null, string status = "", IList<int> ids = null)
        {
            var query = _deviceRepository.Table;

            if (ids != null && ids.Count > 0)
                query = query.Where(d => ids.Contains(d.Id));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(d => d.Status == status);

            if (createdAtMin != null)
                query = query.Where(d => d.CreatedOnUtc > createdAtMin.Value);

            if (createdAtMax != null)
                query = query.Where(d => d.CreatedOnUtc < createdAtMax.Value);

            query = query.OrderBy(d => d.Id);

            return query;
        }
    }
}