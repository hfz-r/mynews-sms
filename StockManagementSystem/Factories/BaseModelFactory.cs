using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Domain.Logging;
using StockManagementSystem.Core.Plugins;
using StockManagementSystem.Services;
using StockManagementSystem.Services.Helpers;
using StockManagementSystem.Services.Logging;
using StockManagementSystem.Services.Plugins;
using StockManagementSystem.Services.Stores;
using StockManagementSystem.Services.Users;

namespace StockManagementSystem.Factories
{
    public class BaseModelFactory : IBaseModelFactory
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPluginService _pluginService;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly IStoreService _storeService;

        public BaseModelFactory(
            IDateTimeHelper dateTimeHelper, 
            IPluginService pluginService, 
            IUserActivityService userActivityService, 
            IUserService userService,
            IStoreService storeService)
        {
            _dateTimeHelper = dateTimeHelper;
            _pluginService = pluginService;
            _userActivityService = userActivityService;
            _userService = userService;
            _storeService = storeService;
        }

        #region Utilities

        /// <summary>
        /// Prepare default item
        /// </summary>
        /// <param name="items">Available items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use "All" text</param>
        protected void PrepareDefaultItem(IList<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (!withSpecialDefaultItem)
                return;

            //use "0" as the default value
            const string value = "0";

            defaultItemText = defaultItemText ?? "All";

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = value });
        }

        #endregion

        /// <summary>
        /// Prepare available activity log types
        /// </summary>
        public async Task PrepareActivityLogTypes(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var availableActivityTypes = await _userActivityService.GetAllActivityTypesAsync();
            foreach (var activityType in availableActivityTypes)
            {
                items.Add(new SelectListItem { Value = activityType.Id.ToString(), Text = activityType.Name });
            }

            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available roles
        /// </summary>
        public Task PrepareRoles(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var availableRoles = _userService.GetRoles();
            foreach (var role in availableRoles)
            {
                items.Add(new SelectListItem { Value = role.Id.ToString(), Text = role.Name });
            }

            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);

            return Task.CompletedTask;
        }

        public Task PrepareTimeZones(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var availableTimeZones = _dateTimeHelper.GetSystemTimeZones();
            foreach (var timeZone in availableTimeZones)
            {
                items.Add(new SelectListItem { Value = timeZone.Id, Text = timeZone.DisplayName });
            }

            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Prepare available load plugin modes
        /// </summary>
        public Task PrepareLoadPluginModes(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var availableLoadPluginModeItems = LoadPluginsMode.All.ToSelectList(false);
            foreach (var loadPluginModeItem in availableLoadPluginModeItems)
            {
                items.Add(loadPluginModeItem);
            }

            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Prepare available plugin groups
        /// </summary>
        public Task PreparePluginGroups(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var availablePluginGroups = _pluginService.GetPluginDescriptors<IPlugin>(LoadPluginsMode.All)
                .Select(plugin => plugin.Group).Distinct().OrderBy(groupName => groupName).ToList();
            foreach (var group in availablePluginGroups)
            {
                items.Add(new SelectListItem { Value = group, Text = group });
            }

            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Prepare available stores
        /// </summary>
        public async Task PrepareStores(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var availableStores = await _storeService.GetStores();
            foreach (var store in availableStores)
            {
                items.Add(new SelectListItem
                {
                    Value = store.P_BranchNo.ToString(),
                    Text = store.P_BranchNo + " - " + store.P_Name
                });
            }

            //PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available store area codes
        /// </summary>
        public async Task PrepareStoreAreaCodes(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var stores = await _storeService.GetStores();

            var availableAreaCodes = stores.Select(store => store.P_AreaCode).Distinct()
                .OrderBy(areacode => areacode).ToList();
            foreach (var areaCode in availableAreaCodes.Where(x => !string.IsNullOrEmpty(x)))
            {
                items.Add(new SelectListItem {Value = areaCode, Text = areaCode});
            }

            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available store cities
        /// </summary>
        public async Task PrepareStoreCities(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var stores = await _storeService.GetStores();

            var availableCities = stores.Select(store => store.P_City).Distinct()
                .OrderBy(city => city).ToList();
            foreach (var city in availableCities.Where(x => !string.IsNullOrEmpty(x)))
            {
                items.Add(new SelectListItem { Value = city, Text = city });
            }

            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available store states
        /// </summary>
        public async Task PrepareStoreStates(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var stores = await _storeService.GetStores();

            var availableStates = stores.Select(store => store.P_State).Distinct()
                .OrderBy(state => state).ToList();
            foreach (var state in availableStates.Where(x => !string.IsNullOrEmpty(x)))
            {
                items.Add(new SelectListItem { Value = state, Text = state });
            }

            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available log levels
        /// </summary>
        public Task PrepareLogLevels(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available log levels
            var availableLogLevelItems = LogLevel.Debug.ToSelectList(false);
            foreach (var logLevelItem in availableLogLevelItems)
            {
                items.Add(logLevelItem);
            }

            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);

            return Task.CompletedTask;
        }
    }
}