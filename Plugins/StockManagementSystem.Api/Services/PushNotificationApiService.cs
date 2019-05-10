using System;
using System.Collections.Generic;
using System.Linq;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.DataStructures;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.PushNotifications;

namespace StockManagementSystem.Api.Services
{
    public class PushNotificationApiService : IPushNotificationApiService
    {
        private readonly IRepository<PushNotification> _pushNotificationRepository;

        public PushNotificationApiService(IRepository<PushNotification> pushNotificationRepository)
        {
            _pushNotificationRepository = pushNotificationRepository;
        }

        #region Private methods

        private IQueryable<PushNotification> GetPushNotificationQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null, IList<int> storeIds = null)
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

            query = query.OrderBy(ol => ol.Id);

            return query;
        }

        #endregion

        public IList<PushNotification> GetPushNotifications(
            DateTime? createdAtMin = null,
            DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId,
            IList<int> storeIds = null)
        {
            var query = GetPushNotificationQuery(createdAtMin, createdAtMax, storeIds);

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
    }
}