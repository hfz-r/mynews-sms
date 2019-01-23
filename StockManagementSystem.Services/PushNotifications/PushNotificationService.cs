using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.PushNotifications;
using StockManagementSystem.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagementSystem.Services.PushNotifications
{
    public class PushNotificationService : IPushNotificationService
    {
        private readonly IRepository<PushNotification> _pushNotificationRepository;
        private readonly IRepository<PushNotificationStore> _pushNotificationStoreRepository;
        private readonly IRepository<NotificationCategory> _notificationCategoryRepository;
        private readonly IRepository<Store> _storeRepository;

        public PushNotificationService(
            IRepository<PushNotification> pushNotificationRepository,
            IRepository<PushNotificationStore> pushNotificationStoreRepository,
            IRepository<NotificationCategory> notificationCategoryRepository,
            IRepository<Store> storeRepository)
        {
            _pushNotificationRepository = pushNotificationRepository;
            _pushNotificationStoreRepository = pushNotificationStoreRepository;
            _notificationCategoryRepository = notificationCategoryRepository;
            _storeRepository = storeRepository;
        }

        public Task<IPagedList<PushNotification>> GetPushNotificationsAsync(
            int[] storeIds = null,
            int[] pushCategoryIds = null,
            string title = null,
            string desc = null,
            string stNo = null, //Stock Take #
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool getOnlyTotalCount = false)
        {
            var query = _pushNotificationRepository.Table;
            var queryStore = _pushNotificationStoreRepository.Table;

            if (storeIds != null && storeIds.Length > 0)
            {
                query = query.Where(pn => pn.PushNotificationStores.Any(pns => storeIds.Contains(pns.StoreId)));
            }

            if (pushCategoryIds != null && pushCategoryIds.Length > 0)
            {
                query = query.Where(pn => pn.PushNotificationStores.Any(pns => pushCategoryIds.Contains(pns.PushNotificationId)));
            }

            if (!string.IsNullOrEmpty(title))
                query = query.Where(u => u.Title.Contains(title));

            if (!string.IsNullOrEmpty(desc))
                query = query.Where(u => u.Desc.Contains(desc));

            if (!string.IsNullOrEmpty(stNo))
                query = query.Where(u => u.StockTakeNo.Contains(stNo));

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return Task.FromResult<IPagedList<PushNotification>>(new PagedList<PushNotification>(query, pageIndex, pageSize,
                getOnlyTotalCount));
        }

        public Task<ICollection<PushNotification>> GetAllPushNotificationsAsync()
        {
            var query = _pushNotificationRepository.Table;

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return Task.FromResult<ICollection<PushNotification>>(new List<PushNotification>(query.ToList()));
        }

        public virtual void UpdatePushNotification(PushNotification pushNotification)
        {
            if (pushNotification == null)
                throw new ArgumentNullException(nameof(pushNotification));

            _pushNotificationRepository.Update(pushNotification);
        }

        public virtual void DeletePushNotification(PushNotification pushNotification)
        { 
            if (pushNotification == null)
                throw new ArgumentNullException(nameof(pushNotification));

            _pushNotificationStoreRepository.Delete(pushNotification.PushNotificationStores);
            _pushNotificationRepository.Delete(pushNotification);
        }

        public virtual void DeletePushNotificationStore(int Id, Store store)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            var query = _pushNotificationStoreRepository.Table.Where(x => x.PushNotificationId == Id && x.StoreId == store.P_BranchNo);

            _pushNotificationStoreRepository.Delete(query);
        }

        public async Task InsertPushNotification(PushNotification pushNotification)
        {
            if (pushNotification == null)
                throw new ArgumentNullException(nameof(pushNotification));

            await _pushNotificationRepository.InsertAsync(pushNotification);
        }

        #region Identity 

        public async Task<PushNotification> GetPushNotificationByIdAsync(int pushNotificationId)
        {
            if (pushNotificationId == 0)
                throw new ArgumentNullException(nameof(pushNotificationId));

            var pushNotification = await _pushNotificationRepository.Table.FirstOrDefaultAsync(u => u.Id == pushNotificationId);
            return pushNotification;
        }

        #endregion

        #region Notification Category

        public async Task<IList<NotificationCategory>> GetNotificationCategoriesAsync()
        {
            var notificationCategory = await _notificationCategoryRepository.Table.ToListAsync();
            return notificationCategory;
        }

        #endregion
    }
}