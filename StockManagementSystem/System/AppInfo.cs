using System.Reflection;
using System.Runtime.InteropServices;

namespace StockManagementSystem.System
{
    public class AppInfo
    {
        public static string GetVersion()
        {
            var assembly = Assembly.GetCallingAssembly();
            return assembly == null ? string.Empty : assembly.GetName().Version.ToString();
        }

        public static string GetDotNetVersion()
        {
            return typeof(RuntimeEnvironment).GetTypeInfo()
                                             .Assembly
                                             .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                             .InformationalVersion;
        }
    }
}
