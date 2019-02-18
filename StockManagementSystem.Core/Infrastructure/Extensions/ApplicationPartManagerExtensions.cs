using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using StockManagementSystem.Core.ComponentModel;
using StockManagementSystem.Core.Configuration;
using StockManagementSystem.Core.Plugins;

namespace StockManagementSystem.Core.Infrastructure.Extensions
{
    public static class ApplicationPartManagerExtensions
    {
        private static readonly IFileProviderHelper _fileProvider;
        private static readonly List<string> _baseAppLibraries;
        private static readonly Dictionary<string, PluginLoadedAssemblyInfo> _loadedAssemblies = new Dictionary<string, PluginLoadedAssemblyInfo>();
        private static readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        static ApplicationPartManagerExtensions()
        {
            //we use the default file provider, since the DI isn't initialized yet
            _fileProvider = CommonHelper.DefaultFileProvider;

            _baseAppLibraries = new List<string>();

            //get all libraries from /bin/{version}/ directory
            _baseAppLibraries.AddRange(_fileProvider.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                .Select(fileName => _fileProvider.GetFileName(fileName)));

            //get all libraries from base site directory
            if (!AppDomain.CurrentDomain.BaseDirectory.Equals(Environment.CurrentDirectory,
                StringComparison.InvariantCultureIgnoreCase))
            {
                _baseAppLibraries.AddRange(_fileProvider.GetFiles(Environment.CurrentDirectory, "*.dll")
                    .Select(fileName => _fileProvider.GetFileName(fileName)));
            }

            //get all libraries from refs directory
            var refsPathName = _fileProvider.Combine(Environment.CurrentDirectory, PluginDefaults.RefsPathName);
            if (_fileProvider.DirectoryExists(refsPathName))
            {
                _baseAppLibraries.AddRange(_fileProvider.GetFiles(refsPathName, "*.dll")
                    .Select(fileName => _fileProvider.GetFileName(fileName)));
            }

            PluginsInfo = PluginsInfo.LoadPluginInfo(_fileProvider);
        }

        private static PluginsInfo PluginsInfo
        {
            get => Singleton<PluginsInfo>.Instance;
            set => Singleton<PluginsInfo>.Instance = value;
        }

        /// <summary>
        /// Copy the plugin assembly file to the shadow copy directory
        /// </summary>
        private static string ShadowCopyFile(IFileProviderHelper fileProvider, string assemblyFile,
            string shadowCopyDirectory)
        {
            var shadowCopiedFile = fileProvider.Combine(shadowCopyDirectory, fileProvider.GetFileName(assemblyFile));

            if (fileProvider.FileExists(shadowCopiedFile))
            {
                //it exists, then check if it's updated (compare creation time of files)
                var areFilesIdentical = fileProvider.GetCreationTime(shadowCopiedFile).ToUniversalTime().Ticks >=
                                        fileProvider.GetCreationTime(assemblyFile).ToUniversalTime().Ticks;
                if (areFilesIdentical)
                {
                    return shadowCopiedFile;
                }

                //file already exists but passed file is more updated, so delete an existing file and copy again
                fileProvider.DeleteFile(shadowCopiedFile);
            }

            try
            {
                fileProvider.FileCopy(assemblyFile, shadowCopiedFile, true);
            }
            catch (IOException)
            {
                //this occurs when the files are locked, rename and re-shadow copy
                try
                {
                    var oldFile = $"{shadowCopiedFile}{Guid.NewGuid():N}.old";
                    fileProvider.FileMove(shadowCopiedFile, oldFile);
                }
                catch (IOException exception)
                {
                    throw new IOException($"{shadowCopiedFile} rename failed, cannot initialize plugin", exception);
                }

                //or retry the shadow copy
                fileProvider.FileCopy(assemblyFile, shadowCopiedFile, true);
            }

            return shadowCopiedFile;
        }

        /// <summary>
        /// Load and register the assembly
        /// </summary>
        private static Assembly AddApplicationParts(ApplicationPartManager applicationPartManager, string assemblyFile , bool useUnsafeLoadAssembly)
        {
            Assembly assembly;

            try
            {
                assembly = Assembly.LoadFrom(assemblyFile);
            }
            catch (FileLoadException)
            {
                if (useUnsafeLoadAssembly)
                {
                    assembly = Assembly.UnsafeLoadFrom(assemblyFile);
                }
                else throw;
            }

            applicationPartManager.ApplicationParts.Add(new AssemblyPart(assembly));

            return assembly;
        }

        /// <summary>
        /// Perform file deploy and return loaded assembly
        /// </summary>
        private static Assembly PerformFileDeploy(this ApplicationPartManager applicationPartManager, string assemblyFile, 
            string shadowCopyDirectory, DefaultConfig config, IFileProviderHelper fileProvider)
        {
            //ensure for proper directory structure
            if (string.IsNullOrEmpty(assemblyFile) || string.IsNullOrEmpty(fileProvider.GetParentDirectory(assemblyFile)))
            {
                throw new InvalidOperationException($"The plugin directory for the {fileProvider.GetFileName(assemblyFile)} file exists in a directory outside of the allowed StockManagementSystem directory hierarchy");
            }

            if (!config.UsePluginsShadowCopy)
            {
                var assembly = AddApplicationParts(applicationPartManager, assemblyFile, config.UseUnsafeLoadAssembly);

                if (assemblyFile.EndsWith(".dll"))
                {
                    _fileProvider.DeleteFile(assemblyFile.Substring(0, assemblyFile.Length - 4) + ".deps.json");
                }

                return assembly;
            }

            fileProvider.CreateDirectory(shadowCopyDirectory);
            var shadowCopiedFile = ShadowCopyFile(fileProvider, assemblyFile, shadowCopyDirectory);

            Assembly shadowCopiedAssembly = null;
            try
            {
                shadowCopiedAssembly = AddApplicationParts(applicationPartManager, shadowCopiedFile, config.UseUnsafeLoadAssembly);
            }
            catch (UnauthorizedAccessException)
            {
                //suppress exceptions for "locked" assemblies, try load them from another directory
                if (!config.CopyLockedPluginAssembilesToSubdirectoriesOnStartup ||
                    !shadowCopyDirectory.Equals(fileProvider.MapPath(PluginDefaults.ShadowCopyPath)))
                {
                    throw;
                }
            }
            catch (FileLoadException)
            {
                //suppress exceptions for "locked" assemblies, try load them from another directory
                if (!config.CopyLockedPluginAssembilesToSubdirectoriesOnStartup ||
                    !shadowCopyDirectory.Equals(fileProvider.MapPath(PluginDefaults.ShadowCopyPath)))
                {
                    throw;
                }
            }

            if (shadowCopiedAssembly != null)
                return shadowCopiedAssembly;

            var reserveDirectory = fileProvider.Combine(fileProvider.MapPath(PluginDefaults.ShadowCopyPath), 
                $"{PluginDefaults.ReserveShadowCopyPathName}{DateTime.Now.ToFileTimeUtc()}");

            return PerformFileDeploy(applicationPartManager, assemblyFile, reserveDirectory, config, fileProvider);
        }

        /// <summary>
        /// Get list of description files-plugin descriptors pairs
        /// </summary>
        private static IList<(string DescriptionFile, PluginDescriptor PluginDescriptor)> GetDescriptionFilesAndDescriptors(string directoryName)
        {
            if (string.IsNullOrEmpty(directoryName))
                throw new ArgumentNullException(nameof(directoryName));

            var result = new List<(string DescriptionFile, PluginDescriptor PluginDescriptor)>();

            var files = _fileProvider.GetFiles(directoryName, PluginDefaults.DescriptionFileName, false);
            foreach (var descriptionFile in files)
            {
                if (!IsPluginDirectory(_fileProvider.GetDirectoryName(descriptionFile)))
                    continue;

                var text = _fileProvider.ReadAllText(descriptionFile, Encoding.UTF8);
                var pluginDescriptor = PluginDescriptor.GetPluginDescriptorFromText(text);

                result.Add((descriptionFile, pluginDescriptor));
            }

            result = result.OrderBy(item => item.PluginDescriptor.SystemName).ToList();

            return result;
        }

        /// <summary>
        /// Check whether the assembly is already loaded
        /// </summary>
        private static bool IsAlreadyLoaded(string filePath, string pluginName)
        {
            var fileName = _fileProvider.GetFileName(filePath);
            if (_baseAppLibraries.Any(library => library.Equals(fileName, StringComparison.InvariantCultureIgnoreCase)))
                return true;

            try
            {
                var fileNameWithoutExtension = _fileProvider.GetFileNameWithoutExtension(filePath);
                if (string.IsNullOrEmpty(fileNameWithoutExtension))
                    throw new Exception($"Cannot get file extension for {fileName}");

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var assemblyName = assembly.FullName.Split(',').FirstOrDefault();
                    if (!fileNameWithoutExtension.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    if (!_loadedAssemblies.ContainsKey(assemblyName))
                    {
                        //add it to the list to find collisions later
                        _loadedAssemblies.Add(assemblyName, new PluginLoadedAssemblyInfo(assemblyName, assembly.FullName));
                    }

                    //set assembly name and plugin name for further using
                    _loadedAssemblies[assemblyName].References.Add((pluginName, AssemblyName.GetAssemblyName(filePath).FullName));

                    return true;
                }
            }
            catch
            {
                // ignored
            }

            //nothing found
            return false;
        }

        /// <summary>
        /// Check whether the directory is a plugin directory
        /// </summary>
        private static bool IsPluginDirectory(string directoryName)
        {
            if (string.IsNullOrEmpty(directoryName))
                return false;

            //get parent directory
            var parent = _fileProvider.GetParentDirectory(directoryName);
            if (string.IsNullOrEmpty(parent))
                return false;

            if (!_fileProvider.GetDirectoryNameOnly(parent).Equals(PluginDefaults.PathName, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }

        public static void InitializePlugins(this ApplicationPartManager applicationPartManager, DefaultConfig config)
        {
            if (applicationPartManager == null)
                throw new ArgumentNullException(nameof(applicationPartManager));

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            //perform with locked access to resources
            using (new WriteLockDisposable(_locker))
            {
                var pluginDescriptors = new List<PluginDescriptor>();
                var incompatiblePlugins = new List<string>();

                try
                {
                    var pluginsDirectory = _fileProvider.MapPath(PluginDefaults.Path);
                    _fileProvider.CreateDirectory(pluginsDirectory);

                    var shadowCopyDirectory = _fileProvider.MapPath(PluginDefaults.ShadowCopyPath);
                    _fileProvider.CreateDirectory(shadowCopyDirectory);

                    var binFiles = _fileProvider.GetFiles(shadowCopyDirectory, "*", false);

                    if (config.ClearPluginShadowDirectoryOnStartup)
                    {
                        var placeholderFileNames = new List<string> { "placeholder.txt", "index.htm" };
                        binFiles = binFiles.Where(file =>
                        {
                            var fileName = _fileProvider.GetFileName(file);
                            return !placeholderFileNames.Any(placeholderFileName => placeholderFileName
                                .Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
                        }).ToArray();

                        foreach (var file in binFiles)
                        {
                            try
                            {
                                _fileProvider.DeleteFile(file);
                            }
                            catch
                            {
                                // ignored
                            }
                        }

                        var reserveDirectories = _fileProvider.GetDirectories(shadowCopyDirectory, PluginDefaults.ReserveShadowCopyPathNamePattern);
                        foreach (var directory in reserveDirectories)
                        {
                            try
                            {
                                _fileProvider.DeleteDirectory(directory);
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    }

                    //load plugin descriptors from the plugin directory
                    foreach (var item in GetDescriptionFilesAndDescriptors(pluginsDirectory))
                    {
                        var descriptionFile = item.DescriptionFile;
                        var pluginDescriptor = item.PluginDescriptor;

                        if (string.IsNullOrEmpty(pluginDescriptor.SystemName?.Trim()))
                        {
                            throw new Exception($"A plugin '{descriptionFile}' has no system name. Try assigning the plugin a unique name and recompiling.");
                        }

                        if (pluginDescriptors.Contains(pluginDescriptor))
                            throw new Exception($"A plugin with '{pluginDescriptor.SystemName}' system name is already defined");

                        //set 'Installed' property
                        pluginDescriptor.Installed = PluginsInfo.InstalledPluginNames.Any(pluginName =>
                            pluginName.Equals(pluginDescriptor.SystemName, StringComparison.InvariantCultureIgnoreCase));

                        try
                        {
                            var pluginDirectory = _fileProvider.GetDirectoryName(descriptionFile);
                            if (string.IsNullOrEmpty(pluginDirectory))
                            {
                                throw new Exception($"Directory cannot be resolved for '{_fileProvider.GetFileName(descriptionFile)}' description file");
                            }

                            //get list of all library files in the plugin directory (not in the bin one)
                            var pluginFiles = _fileProvider.GetFiles(pluginDirectory, "*.dll", false)
                                .Where(file => !binFiles.Contains(file) && IsPluginDirectory(_fileProvider.GetDirectoryName(file)))
                                .ToList();

                            var mainPluginFile = pluginFiles.FirstOrDefault(file =>
                            {
                                var fileName = _fileProvider.GetFileName(file);
                                return fileName.Equals(pluginDescriptor.AssemblyFileName, StringComparison.InvariantCultureIgnoreCase);
                            });

                            if (mainPluginFile == null)
                            {
                                incompatiblePlugins.Add(pluginDescriptor.SystemName);
                                continue;
                            }

                            var pluginName = pluginDescriptor.SystemName;

                            pluginDescriptor.OriginalAssemblyFile = mainPluginFile;

                            //need to deploy if plugin is already installed
                            var needToDeploy = PluginsInfo.InstalledPluginNames.Contains(pluginName);

                            //also, deploy if the plugin is only going to be installed now
                            needToDeploy = needToDeploy || PluginsInfo.PluginNamesToInstall.Any(pluginInfo => pluginInfo.SystemName.Equals(pluginName));

                            //finally, exclude from deploying the plugin that is going to be deleted
                            needToDeploy = needToDeploy && !PluginsInfo.PluginNamesToDelete.Contains(pluginName);

                            if (needToDeploy)
                            {
                                pluginDescriptor.ReferencedAssembly = applicationPartManager.PerformFileDeploy(mainPluginFile, shadowCopyDirectory, config, _fileProvider);

                                var filesToDeploy = pluginFiles.Where(file =>
                                    !_fileProvider.GetFileName(file).Equals(_fileProvider.GetFileName(mainPluginFile)) &&
                                    !IsAlreadyLoaded(file, pluginName)).ToList();
                                foreach (var file in filesToDeploy)
                                {
                                    applicationPartManager.PerformFileDeploy(file, shadowCopyDirectory, config, _fileProvider);
                                }

                                var pluginType = pluginDescriptor.ReferencedAssembly.GetTypes().FirstOrDefault(type =>
                                    typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface && type.IsClass && !type.IsAbstract);
                                if (pluginType != default(Type))
                                    pluginDescriptor.PluginType = pluginType;
                            }

                            if (PluginsInfo.PluginNamesToDelete.Contains(pluginName))
                                continue;

                            pluginDescriptors.Add(pluginDescriptor);
                        }
                        catch (ReflectionTypeLoadException exception)
                        {
                            var error = exception.LoaderExceptions.Aggregate($"Plugin '{pluginDescriptor.FriendlyName}'. ",
                                (message, nextMessage) => $"{message}{nextMessage.Message}{Environment.NewLine}");

                            throw new Exception(error, exception);
                        }
                        catch (Exception exception)
                        {
                            throw new Exception($"Plugin '{pluginDescriptor.FriendlyName}'. {exception.Message}", exception);
                        }
                    }
                }
                catch (Exception exception)
                {
                    var message = string.Empty;
                    for (var inner = exception; inner != null; inner = inner.InnerException)
                        message = $"{message}{inner.Message}{Environment.NewLine}";

                    throw new Exception(message, exception);
                }

                PluginsInfo.PluginDescriptors = pluginDescriptors;

                PluginsInfo.IncompatiblePlugins = incompatiblePlugins;
                PluginsInfo.AssemblyLoadedCollision = _loadedAssemblies.Select(item => item.Value)
                    .Where(loadedAssemblyInfo => loadedAssemblyInfo.Collisions.Any()).ToList();
            }
        }
    }
}