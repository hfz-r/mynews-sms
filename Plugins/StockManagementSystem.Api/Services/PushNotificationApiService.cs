using System;
using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Api.DTOs.PushNotifications;
using StockManagementSystem.Api.Extensions;
using StockManagementSystem.Api.Infrastructure.Mapper.Extensions;
using StockManagementSystem.Api.Models.GenericsParameters;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.PushNotifications;
using static StockManagementSystem.Api.Extensions.ApiServiceExtension;

namespace StockManagementSystem.Api.Services
{
    public class PushNotificationApiService : IPushNotificationApiService
    {
        private readonly IRepository<PushNotification> _pushNotificationRepository;
        private readonly IRepository<PushNotificationStore> _pushNotificationStoreRepository;

        public PushNotificationApiService(
            IRepository<PushNotification> pushNotificationRepository,
            IRepository<PushNotificationStore> pushNotificationStoreRepository)
        {
            _pushNotificationRepository = pushNotificationRepository;
            _pushNotificationStoreRepository = pushNotificationStoreRepository;
        }

        #region Private methods

        private IQueryable<PushNotification> GetPushNotificationQuery(DateTime? createdAtMin = null,
            DateTime? createdAtMax = null, IList<int> storeIds = null, DateTime? startTime = null,
            DateTime? endTime = null)
        {
            var query = _pushNotificationRepository.Table;

            if (createdAtMin != null)
                query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);

            if (createdAtMax != null)
                query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);

            if (storeIds != null && storeIds.Count > 0)
            {
                var result = new List<PushNotification>();

                foreach (var pushNotification in query)
                {
                    var pn = pushNotification;

                    var pnStore = pn.PushNotificationStores.Where(x => storeIds.Contains(x.StoreId)).ToList();
                    pn.PushNotificationStores = pnStore;

                    result.Add(pn);
                }

                query = result.AsQueryable();
            }

            if (startTime != null)
                query = query.Where(c => c.StartTime > startTime.Value);

            if (endTime != null)
                query = query.Where(c => c.EndTime < endTime.Value);

            query = query.OrderBy(ol => ol.Id);

            return query;
        }

        private IQueryable<PushNotification> GetPushNotificationByStoreId(IQueryable<PushNotification> query, int storeId)
        {
            query = query.Join(_pushNotificationStoreRepository.Table, x => x.Id, y => y.PushNotificationId,
                    (x, y) => new {PushNotification = x, PushNotificationStore = y})
                .Where(z => z.PushNotificationStore.StoreId == storeId).Select(pn => pn.PushNotification);

            return query;
        }

        #endregion

        public IList<PushNotification> GetPushNotifications(
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            IList<int> storeIds = null,
            DateTime? startTime = null,
            DateTime? endTime = null)
        {
            var query = GetPushNotificationQuery(createdAtMin, createdAtMax, storeIds, startTime, endTime);

            if (sinceId > 0)
                query = query.Where(ol => ol.Id > sinceId);

            return new ApiList<PushNotification>(query, page - 1, limit);
        }

        public int GetPushNotificationsCount()
        {
            return _pushNotificationRepository.Table.Count();
        }

        public PushNotification GetPushNotificationById(int id)
        {
            if (id == 0)
                return null;

            var pushNotification = _pushNotificationRepository.Table.FirstOrDefault(pn => pn.Id == id);

            return pushNotification;
        }

        public Search<PushNotificationDto> Search(
            int storeId = 0,
            string queryParams = "",
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            string sortColumn = Configurations.DefaultOrder,
            bool descending = false,
            bool count = false)
        {
            var query = _pushNotificationRepository.Table;

            if (storeId > 0)
                query = GetPushNotificationByStoreId(query, storeId);

            var searchParams = EnsureSearchQueryIsValid(queryParams, ResolveSearchQuery);
            if (searchParams != null)
            {
                query = query.HandleSearchParams(searchParams);
            }

            query = query.GetQueryDynamic(sortColumn, @descending);

            var _ = new SearchWrapper<PushNotificationDto, PushNotification>();
            return count
                ? _.ToCount(query)
                : _.ToList(query, page, limit,
                    list => list.Select(entity => entity.ToDto()).ToList() as IList<PushNotificationDto>);
        }
    }
}