using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using StockManagementSystem.Core.Configuration;
using StockManagementSystem.Core.Domain.Configuration;

namespace StockManagementSystem.Services.Configuration
{
    public interface ISettingService
    {
        void ClearCache();

        Task DeleteSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int storeId = 0) where T : ISettings, new();

        Task DeleteSetting<T>() where T : ISettings, new();

        Task DeleteSettingAsync(Setting setting);

        Task DeleteSettingsAsync(IList<Setting> settings);

        Task<IList<Setting>> GetAllSettingsAsync();

        Task<Setting> GetSettingAsync(string key, int storeId = 0, bool loadSharedValueIfNotFound = false);

        Task<Setting> GetSettingByIdAsync(int settingId);

        T GetSettingByKey<T>(string key, T defaultValue = default(T), int storeId = 0, bool loadSharedValueIfNotFound = false);

        string GetSettingKey<TSettings, T>(TSettings settings, Expression<Func<TSettings, T>> keySelector) where TSettings : ISettings, new();

        Task InsertSettingAsync(Setting setting, bool clearCache = true);

        ISettings LoadSetting(Type type, int storeId = 0);

        T LoadSetting<T>(int storeId = 0) where T : ISettings, new();

        void SaveSetting<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int storeId = 0, bool clearCache = true) where T : ISettings, new();

        void SaveSetting<T>(T settings, int storeId = 0) where T : ISettings, new();

        void SetSetting<T>(string key, T value, int storeId = 0, bool clearCache = true);

        bool SettingExists<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, int storeId = 0) where T : ISettings, new();

        Task SaveSettingOverridablePerStore<T, TPropType>(T settings, Expression<Func<T, TPropType>> keySelector, bool overrideForStore, int storeId = 0, bool clearCache = true) where T : ISettings, new();
    }
}