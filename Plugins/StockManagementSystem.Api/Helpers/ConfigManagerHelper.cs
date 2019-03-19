using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using StockManagementSystem.Core.Data;

namespace StockManagementSystem.Api.Helpers
{
    public class ConfigManagerHelper : IConfigManagerHelper
    {
        public ConfigManagerHelper(DataSettings dataSettings)
        {
            DataSettings = dataSettings;
        }

        public ConfigManagerHelper()
        {
            DataSettings = DataSettingsManager.LoadSettings();
        }

        public DataSettings DataSettings { get; }

        public void AddBindingRedirects()
        {
            var hasChanged = false;

            // load SMS.exe.config
            XDocument appConfig = null;

            var webAssemblyConfigLocation = $"{Assembly.GetEntryAssembly().Location}.config";

            using (var fs = File.OpenRead(webAssemblyConfigLocation))
            {
                appConfig = XDocument.Load(fs);
            }

            appConfig.Changed += (o, e) => { hasChanged = true; };

            var runtime = appConfig.XPathSelectElement("configuration//runtime");
            if (runtime == null)
            {
                runtime = new XElement("runtime");
                appConfig.XPathSelectElement("configuration")?.Add(runtime);
            }

            if (hasChanged)
            {
                try
                {
                    appConfig.Save(webAssemblyConfigLocation);
                }
                catch (Exception)
                {
                    //do nothing
                }
            }
        }

        public void AddConnectionString()
        {
            var hasChanged = false;

            // load web.config
            XDocument appConfig = null;

            var webAssemblyConfigLocation = $"{Assembly.GetEntryAssembly().Location}.config";

            using (var fs = File.OpenRead(webAssemblyConfigLocation))
            {
                appConfig = XDocument.Load(fs);
            }

            appConfig.Changed += (o, e) => { hasChanged = true; };

            var connectionStrings = appConfig.XPathSelectElement("configuration//connectionStrings");
            if (connectionStrings == null)
            {
                var configuration = appConfig.XPathSelectElement("configuration");
                connectionStrings = new XElement("connectionStrings");
                configuration.Add(connectionStrings);
            }

            var connStr = DataSettings.DataConnectionString;

            var element = appConfig.XPathSelectElement("configuration//connectionStrings//add[@name='MS_SqlStoreConnectionString']");
            if (element == null)
            {
                element = new XElement("add");
                element.SetAttributeValue("name", "MS_SqlStoreConnectionString");
                element.SetAttributeValue("connectionString", connStr);
                element.SetAttributeValue("providerName", "System.Data.SqlClient");
                connectionStrings.Add(element);
            }
            else
            {
                var connectionStringInConfig = element.Attribute("connectionString")?.Value;

                if (!string.Equals(connStr, connectionStringInConfig, StringComparison.InvariantCultureIgnoreCase))
                    element.SetAttributeValue("connectionString", connStr);
            }

            if (hasChanged)
            {
                try
                {
                    appConfig.Save(webAssemblyConfigLocation);
                }
                catch (Exception)
                {
                    //do nothing
                }
            }
        }
    }
}