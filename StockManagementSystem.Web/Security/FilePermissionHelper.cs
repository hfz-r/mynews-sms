using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Core.Plugins;

namespace StockManagementSystem.Web.Security
{
    public static class FilePermissionHelper
    {
        private static void CheckAccessRule(
            FileSystemAccessRule rule,
            ref bool deleteIsDeny,
            ref bool modifyIsDeny,
            ref bool readIsDeny,
            ref bool writeIsDeny,
            ref bool deleteIsAllow,
            ref bool modifyIsAllow,
            ref bool readIsAllow,
            ref bool writeIsAllow)
        {
            bool CheckAccessRule(FileSystemAccessRule fileSystemAccessRule, FileSystemRights fileSystemRights)
            {
                return (fileSystemRights & fileSystemAccessRule.FileSystemRights) == fileSystemRights;
            }

            switch (rule.AccessControlType)
            {
                case AccessControlType.Deny:
                    if (CheckAccessRule(rule, FileSystemRights.Delete))
                        deleteIsDeny = true;

                    if (CheckAccessRule(rule, FileSystemRights.Modify))
                        modifyIsDeny = true;

                    if (CheckAccessRule(rule, FileSystemRights.Read))
                        readIsDeny = true;

                    if (CheckAccessRule(rule, FileSystemRights.Write))
                        writeIsDeny = true;

                    return;
                case AccessControlType.Allow:
                    if (CheckAccessRule(rule, FileSystemRights.Delete))
                        deleteIsAllow = true;

                    if (CheckAccessRule(rule, FileSystemRights.Modify))
                        modifyIsAllow = true;

                    if (CheckAccessRule(rule, FileSystemRights.Read))
                        readIsAllow = true;

                    if (CheckAccessRule(rule, FileSystemRights.Write))
                        writeIsAllow = true;
                    break;
            }
        }

        public static bool CheckPermissions(string path, bool checkRead, bool checkWrite, bool checkModify, bool checkDelete)
        {
            var permissionsAreGranted = true;

            try
            {
                var fileProvider = EngineContext.Current.Resolve<IFileProviderHelper>();

                if (!(fileProvider.FileExists(path) || fileProvider.DirectoryExists(path)))
                {
                    return true;
                }

                var current = WindowsIdentity.GetCurrent();

                var readIsDeny = false;
                var writeIsDeny = false;
                var modifyIsDeny = false;
                var deleteIsDeny = false;

                var readIsAllow = false;
                var writeIsAllow = false;
                var modifyIsAllow = false;
                var deleteIsAllow = false;

                var rules = fileProvider.GetAccessControl(path).GetAccessRules(true, true, typeof(SecurityIdentifier))
                    .Cast<FileSystemAccessRule>().ToList();

                foreach (var rule in rules.Where(rule => current.User?.Equals(rule.IdentityReference) ?? false))
                {
                    CheckAccessRule(rule, ref deleteIsDeny, ref modifyIsDeny, ref readIsDeny, ref writeIsDeny,
                        ref deleteIsAllow, ref modifyIsAllow, ref readIsAllow, ref writeIsAllow);
                }

                if (current.Groups != null)
                {
                    foreach (var reference in current.Groups)
                    {
                        foreach (var rule in rules.Where(rule => reference.Equals(rule.IdentityReference)))
                        {
                            CheckAccessRule(rule, ref deleteIsDeny, ref modifyIsDeny, ref readIsDeny, ref writeIsDeny,
                                ref deleteIsAllow, ref modifyIsAllow, ref readIsAllow, ref writeIsAllow);
                        }
                    }
                }

                deleteIsAllow = !deleteIsDeny && deleteIsAllow;
                modifyIsAllow = !modifyIsDeny && modifyIsAllow;
                readIsAllow = !readIsDeny && readIsAllow;
                writeIsAllow = !writeIsDeny && writeIsAllow;

                if (checkRead)
                    permissionsAreGranted = readIsAllow;

                if (checkWrite)
                    permissionsAreGranted = permissionsAreGranted && writeIsAllow;

                if (checkModify)
                    permissionsAreGranted = permissionsAreGranted && modifyIsAllow;

                if (checkDelete)
                    permissionsAreGranted = permissionsAreGranted && deleteIsAllow;
            }
            catch (System.IO.IOException)
            {
                return false;
            }
            catch
            {
                return true;
            }

            return permissionsAreGranted;
        }

        public static IEnumerable<string> GetDirectoriesWrite()
        {
            var fileProvider = EngineContext.Current.Resolve<IFileProviderHelper>();

            var rootDir = fileProvider.MapPath("~/");

            var dirsToCheck = new List<string>
            {
                fileProvider.Combine(rootDir, "App_Data"),
                fileProvider.Combine(rootDir, "bin"),
                fileProvider.Combine(rootDir, "plugins"),
                fileProvider.Combine(rootDir, "plugins\\bin"),
                fileProvider.Combine(rootDir, "wwwroot\\bundles"),
                fileProvider.Combine(rootDir, "wwwroot\\images"),
            };

            return dirsToCheck;
        }

        public static IEnumerable<string> GetFilesWrite()
        {
            var fileProvider = EngineContext.Current.Resolve<IFileProviderHelper>();

            return new List<string>
            {
                fileProvider.MapPath(PluginDefaults.InstalledPluginsFilePath),
                fileProvider.MapPath(DataSettingsDefaults.FilePath)
            };
        }
    }
}