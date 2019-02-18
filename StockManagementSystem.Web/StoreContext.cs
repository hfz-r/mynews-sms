using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Stores;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Services.Common;
using StockManagementSystem.Services.Stores;

namespace StockManagementSystem.Web
{
    public class StoreContext : IStoreContext
    {
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStoreService _storeService;

        private Store _cachedStore;
        private int? _cachedActiveStoreScopeConfiguration;

        public StoreContext(
            IGenericAttributeService genericAttributeService, 
            IHttpContextAccessor httpContextAccessor, 
            IStoreService storeService)
        {
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _storeService = storeService;
        }

        public virtual Store CurrentStore
        {
            get
            {
                if (_cachedStore != null)
                    return _cachedStore;

                //try to determine the current store by HOST header
                string host = _httpContextAccessor.HttpContext?.Request?.Headers[HeaderNames.Host];

                var allStores = _storeService.GetStoresAsync().GetAwaiter().GetResult();
                var store = allStores.FirstOrDefault(s => _storeService.ContainsHostValue(s, host)) ??
                            allStores.FirstOrDefault();

                _cachedStore = store ?? throw new Exception("No store could be loaded");

                return _cachedStore;
            }
        }

        public virtual int ActiveStoreScopeConfiguration
        {
            get
            {
                if (_cachedActiveStoreScopeConfiguration.HasValue)
                    return _cachedActiveStoreScopeConfiguration.Value;

                //ensure that we have 2 (or more) stores
                if (_storeService.GetStoresAsync().GetAwaiter().GetResult().Count > 1)
                {
                    var currentUser = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;

                    //try to get store identifier from attributes
                    var branchNo = _genericAttributeService
                        .GetAttributeAsync<int>(currentUser, UserDefaults.StoreScopeConfigurationAttribute)
                        .GetAwaiter().GetResult();

                    _cachedActiveStoreScopeConfiguration =
                        _storeService.GetStoreByBranchNoAsync(branchNo).GetAwaiter().GetResult()?.P_BranchNo ?? 0;
                }
                else
                    _cachedActiveStoreScopeConfiguration = 0;

                return _cachedActiveStoreScopeConfiguration ?? 0;
            }
        }
    }
}