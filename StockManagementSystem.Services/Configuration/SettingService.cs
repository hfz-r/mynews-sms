using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Configuration;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Configuration;

namespace StockManagementSystem.Services.Configuration
{
    /// <summary>
    /// Setting manager
    /// </summary>
    public class SettingService : ISettingService
    {
        private readonly IRepository<Setting> _settingRepository;
        private readonly IStaticCacheManager _cacheManager;

        public SettingService(IRepository<Setting> settingRepository, IStaticCacheManager cacheManager)
        {
            _settingRepository = settingRepository;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// Setting (for caching)
        /// </summary>
        [Serializable]
        public class SettingForCaching
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public string Value { get; set; }

            public int TenantId { get; set; }
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        protected IDictionary<string, IList<SettingForCaching>> GetAllSettingsCached()
        {
            return _cacheManager.Get(ConfigurationDefaults.SettingsAllCacheKey, () =>
            {
                //use no tracking here for performance optimization
                //anyway records are loaded only for read-only operations
                var query = from s in _settingRepository.TableNoTracking
                    orderby s.Name, s.TenantId select s;
                var settings = query.ToList();
                var dictionary = new Dictionary<string, IList<SettingForCaching>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new SettingForCaching
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value,
                        TenantId = s.TenantId,
                    };
                    if (!dictionary.ContainsKey(resourceName))
                    {
                        dictionary.Add(resourceName, new List<SettingForCaching>
                        {
                            settingForCaching
                        });
                    }
                    else
                    {
                        //already added
                        dictionary[resourceName].Add(settingForCaching);
                    }
                }

                return dictionary;
            });
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        protected void SetSetting(Type type, string key, object value, int tenantId = 0, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            key = key.Trim().ToLowerInvariant();

            var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key)
                ? allSettings[key].FirstOrDefault(x => x.TenantId == tenantId) : null;
            if (settingForCaching != null)
            {
                //update
                var setting = GetSettingByIdAsync(settingForCaching.Id).GetAwaiter().GetResult();
                setting.Value = valueStr;
                UpdateSettingAsync(setting, clearCache).GetAwaiter().GetResult();
            }
            else
            {
                //insert
                var setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                    TenantId = tenantId,
                };
                InsertSettingAsync(setting, clearCache).GetAwaiter().GetResult();
            }
        }

        public async Task InsertSettingAsync(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            await _settingRepository.InsertAsync(setting);

            if (clearCache)
                _cacheManager.RemoveByPattern(ConfigurationDefaults.SettingsPatternCacheKey);
        }

        public async Task UpdateSettingAsync(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            await _settingRepository.UpdateAsync(setting);

            if (clearCache)
                _cacheManager.RemoveByPattern(ConfigurationDefaults.SettingsPatternCacheKey);
        }

        public virtual async Task DeleteSettingAsync(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            await _settingRepository.DeleteAsync(setting);

            _cacheManager.RemoveByPattern(ConfigurationDefaults.SettingsPatternCacheKey);
        }

        public async Task DeleteSettingsAsync(IList<Setting> settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            await _settingRepository.DeleteAsync(settings);

            _cacheManager.RemoveByPattern(ConfigurationDefaults.SettingsPatternCacheKey);
        }

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        public virtual async Task<Setting> GetSettingByIdAsync(int settingId)
        {
            if (settingId == 0)
                return null;

            return await _settingRepository.GetByIdAsync(settingId);
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        public async Task<Setting> GetSettingAsync(string key, int tenantId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(key))
                return null;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault(x => x.TenantId == tenantId);

            if (setting == null && tenantId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.TenantId == 0);

            return setting != null ? await GetSettingByIdAsync(setting.Id) : null;
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T), int tenantId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsCached();
            key = key.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(key))
                return defaultValue;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault(x => x.TenantId == tenantId);

            if (setting == null && tenantId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.TenantId == 0);

            return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        public virtual void SetSetting<T>(string key, T value, int tenantId = 0, bool clearCache = true)
        {
            SetSetting(typeof(T), key, value, tenantId, clearCache);
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        public virtual async Task<IList<Setting>> GetAllSettingsAsync()
        {
            var query = from s in _settingRepository.Table
                orderby s.Name, s.TenantId
                select s;
            var settings = await query.ToListAsync();
            return settings;
        }

        /// <summary>
        /// Determines whether a setting exists
        /// </summary>
        public bool SettingExists<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int tenantId = 0)
            where T : ISettings, new()
        {
            var key = GetSettingKey(settings, keySelector);

            var setting = GetSettingByKey<string>(key, tenantId: tenantId);

            return setting != null;
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="tenantId">Tenant identifier for which settings should be loaded</param>
        public T LoadSetting<T>(int tenantId = 0) where T : ISettings, new()
        {
            return (T) LoadSetting(typeof(T), tenantId);
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="tenantId">Tenant identifier for which settings should be loaded</param>
        public ISettings LoadSetting(Type type, int tenantId = 0)
        {
            var settings = Activator.CreateInstance(type);

            foreach (var prop in type.GetProperties())
            {
                // get properties which can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = type.Name + "." + prop.Name;
                //load by tenant
                var setting = GetSettingByKey<string>(key, tenantId: tenantId, loadSharedValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                    continue;

                var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings as ISettings;
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="tenantId">Tenant identifier</param>
        /// <param name="settings">Setting instance</param>
        public void SaveSetting<T>(T settings, int tenantId = 0) where T : ISettings, new()
        {
            /* cache will not be clear after each setting update = increased performance */
            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                var value = prop.GetValue(settings, null);
                if (value != null)
                    SetSetting(prop.PropertyType, key, value, tenantId, false);
                else
                    SetSetting(key, string.Empty, tenantId, false);
            }

            //and now clear cache
            ClearCache();
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public void SaveSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int tenantId = 0,
            bool clearCache = true) where T : ISettings, new()
        {
            if (!(keySelector.Body is MemberExpression member))
            {
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");
            }

            var key = GetSettingKey(settings, keySelector);
            var value = (TPropType) propInfo.GetValue(settings, null);
            if (value != null)
                SetSetting(key, value, tenantId, clearCache);
            else
                SetSetting(key, string.Empty, tenantId, clearCache);
        }

        /// <summary>
        /// Save settings object (per tenant). If the setting is not overridden per tenant then it'll be delete
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="overrideForTenant">A value indicating whether to setting is overridden in some tenant</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public async Task SaveSettingOverridablePerTenant<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector,
            bool overrideForTenant, int tenantId = 0, bool clearCache = true) where T : ISettings, new()
        {
            if (overrideForTenant || tenantId == 0)
                SaveSetting(settings, keySelector, tenantId, clearCache);
            else if (tenantId > 0)
                await DeleteSetting(settings, keySelector, tenantId);
        }

        /// <summary>
        /// Delete all settings
        /// </summary>
        public async Task DeleteSetting<T>() where T : ISettings, new()
        {
            var settingsToDelete = new List<Setting>();
            var allSettings = await GetAllSettingsAsync();
            foreach (var prop in typeof(T).GetProperties())
            {
                var key = typeof(T).Name + "." + prop.Name;
                settingsToDelete.AddRange(allSettings.Where(x =>
                    x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
            }

            await DeleteSettingsAsync(settingsToDelete);
        }

        /// <summary>
        /// Delete settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="tenantId">Tenant ID</param>
        public async Task DeleteSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int tenantId = 0)
            where T : ISettings, new()
        {
            var key = GetSettingKey(settings, keySelector);
            key = key.Trim().ToLowerInvariant();

            var allSettings = GetAllSettingsCached();
            var settingForCaching = allSettings.ContainsKey(key) ? allSettings[key].FirstOrDefault(x => x.TenantId == tenantId) : null;
            if (settingForCaching == null)
                return;

            //update
            var setting = await GetSettingByIdAsync(settingForCaching.Id);
            await DeleteSettingAsync(setting);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            _cacheManager.RemoveByPattern(ConfigurationDefaults.SettingsPatternCacheKey);
        }

        /// <summary>
        /// Get setting key (stored into database)
        /// </summary>
        /// <typeparam name="TSettings">Type of settings</typeparam>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <returns>Key</returns>
        public string GetSettingKey<TSettings, T>(TSettings settings, Expression<Func<TSettings, T>> keySelector)
            where TSettings : ISettings, new()
        {
            if (!(keySelector.Body is MemberExpression member))
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            if (!(member.Member is PropertyInfo propInfo))
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var key = $"{typeof(TSettings).Name}.{propInfo.Name}";

            return key;
        }
    }
}