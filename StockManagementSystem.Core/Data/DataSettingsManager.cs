using System;
using System.Text;
using Newtonsoft.Json;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Core.Data
{
    public partial class DataSettingsManager
    {
        private static bool? _databaseIsInstalled;

        public static DataSettings LoadSettings(string filePath = null, bool reloadSettings = false, IFileProviderHelper fileProvider = null)
        {
            if (!reloadSettings && Singleton<DataSettings>.Instance != null)
                return Singleton<DataSettings>.Instance;

            fileProvider = fileProvider ?? CommonHelper.DefaultFileProvider;

            filePath = filePath ?? fileProvider.MapPath(DataSettingsDefaults.FilePath);
            if (!fileProvider.FileExists(filePath))
                return new DataSettings();

            var text = fileProvider.ReadAllText(filePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
                return new DataSettings();

            //get data settings from the JSON file
            Singleton<DataSettings>.Instance = JsonConvert.DeserializeObject<DataSettings>(text);

            return Singleton<DataSettings>.Instance;
        }

        public static void SaveSettings(DataSettings settings, IFileProviderHelper fileProvider = null)
        {
            Singleton<DataSettings>.Instance = settings ?? throw new ArgumentNullException(nameof(settings));

            fileProvider = fileProvider ?? CommonHelper.DefaultFileProvider;
            var filePath = fileProvider.MapPath(DataSettingsDefaults.FilePath);

            //create file if not exists
            fileProvider.CreateFile(filePath);

            //save data settings to the file
            var text = JsonConvert.SerializeObject(Singleton<DataSettings>.Instance, Formatting.Indented);
            fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
        }

        public static void ResetCache()
        {
            _databaseIsInstalled = null;
        }

        #region Properties

        public static bool DatabaseIsInstalled
        {
            get
            {
                if (!_databaseIsInstalled.HasValue)
                    _databaseIsInstalled =
                        !string.IsNullOrEmpty(LoadSettings(reloadSettings: true)?.DataConnectionString);

                return _databaseIsInstalled.Value;
            }
        }

        #endregion
    }
}