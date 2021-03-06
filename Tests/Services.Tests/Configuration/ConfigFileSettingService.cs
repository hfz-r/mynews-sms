﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Domain.Configuration;
using StockManagementSystem.Services.Configuration;
using StockManagementSystem.Services.Events;

namespace Services.Tests.Configuration
{
    public class ConfigFileSettingService : SettingService
    {
        public ConfigFileSettingService(IEventPublisher eventPublisher, IRepository<Setting> settingRepository,
            IStaticCacheManager cacheManager) :
            base(eventPublisher, settingRepository, cacheManager)
        {
        }

        public override Task<Setting> GetSettingByIdAsync(int settingId)
        {
            throw new InvalidOperationException("Get setting by id is not supported");
        }

        public override T GetSettingByKey<T>(string key, T defaultValue = default(T), int tenantId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsAsync().GetAwaiter().GetResult();
            key = key.Trim().ToLowerInvariant();

            var setting = settings.FirstOrDefault(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase) && x.TenantId == tenantId);
            if (setting == null && tenantId > 0 && loadSharedValueIfNotFound)
            {
                setting = settings.FirstOrDefault(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase) && x.TenantId == 0);
            }

            if (setting != null)
                return CommonHelper.To<T>(setting.Value);

            return defaultValue;
        }

        public override Task DeleteSettingAsync(Setting setting)
        {
            throw new InvalidOperationException("Deleting settings is not supported");
        }

        public override void SetSetting<T>(string key, T value, int tenantId = 0, bool clearCache = true)
        {
            throw new NotImplementedException();
        }

        public override Task<IList<Setting>> GetAllSettingsAsync()
        {
            var settings = new List<Setting>();
            var appSettings = new Dictionary<string, string>
            {
                { "Setting1", "SomeValue"},
                { "Setting2", "25"},
                { "Setting3", "12/25/2010"},
                { "TestSettings.ServerName", "Ruby"},
                { "TestSettings.Ip", "192.168.0.1"},
                { "TestSettings.PortNumber", "21"},
                { "TestSettings.Username", "admin"},
                { "TestSettings.Password", "password"}
            };

            foreach (var setting in appSettings)
            {
                settings.Add(new Setting
                {
                    Name = setting.Key.ToLowerInvariant(),
                    Value = setting.Value
                });
            }

            return Task.FromResult<IList<Setting>>(settings);
        }

        public override void ClearCache()
        {
        }
    }
}