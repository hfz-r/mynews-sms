using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BundlerMinifier;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using StockManagementSystem.Core.Caching;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Web.UI
{
    /// <summary>
    /// Page head builder
    /// </summary>
    public partial class PageHeadBuilder : IPageHeadBuilder
    {
        private static readonly object s_lock = new object();

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IFileProviderHelper _fileProvider;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly BundleFileProcessor _processor;

        private readonly List<string> _titleParts;
        private readonly Dictionary<ResourceLocation, List<ScriptReferenceMeta>> _scriptParts;
        private readonly Dictionary<ResourceLocation, List<string>> _inlineScriptParts;
        private readonly Dictionary<ResourceLocation, List<CssReferenceMeta>> _cssParts;
        private readonly List<string> _pageCssClassParts;
        private string _editPageUrl;
        private string _activeAdminMenuSystemName;

        public PageHeadBuilder(
            IHostingEnvironment hostingEnvironment,
            IFileProviderHelper fileProvider,
            IStaticCacheManager staticCacheManager)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._fileProvider = fileProvider;
            this._staticCacheManager = staticCacheManager;
            this._processor = new BundleFileProcessor();

            this._titleParts = new List<string>();
            this._scriptParts = new Dictionary<ResourceLocation, List<ScriptReferenceMeta>>();
            this._inlineScriptParts = new Dictionary<ResourceLocation, List<string>>();
            this._cssParts = new Dictionary<ResourceLocation, List<CssReferenceMeta>>();
            this._pageCssClassParts = new List<string>();
        }

        #region Utilities

        /// <summary>
        /// Get bundled file name
        /// </summary>
        /// <param name="parts">Parts to bundle</param>
        /// <returns>File name</returns>
        protected virtual string GetBundleFileName(string[] parts)
        {
            if (parts == null || parts.Length == 0)
                throw new ArgumentException("parts");

            //calculate hash
            var hash = "";
            using (SHA256 sha = new SHA256Managed())
            {
                // string concatenation
                var hashInput = "";
                foreach (var part in parts)
                {
                    hashInput += part;
                    hashInput += ",";
                }

                var input = sha.ComputeHash(Encoding.Unicode.GetBytes(hashInput));
                hash = WebEncoders.Base64UrlEncode(input);
            }

            return hash;
        }

        #endregion

        /// <summary>
        /// Add title element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="part">Title part</param>
        public virtual void AddTitleParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _titleParts.Add(part);
        }

        /// <summary>
        /// Add script element
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        /// <param name="isAsync">A value indicating whether to add an attribute "async" or not for js files</param>
        public virtual void AddScriptParts(ResourceLocation location, string src, string debugSrc,
            bool excludeFromBundle, bool isAsync)
        {
            if (!_scriptParts.ContainsKey(location))
                _scriptParts.Add(location, new List<ScriptReferenceMeta>());

            if (string.IsNullOrEmpty(src))
                return;

            if (string.IsNullOrEmpty(debugSrc))
                debugSrc = src;

            _scriptParts[location].Add(new ScriptReferenceMeta
            {
                ExcludeFromBundle = excludeFromBundle,
                IsAsync = isAsync,
                Src = src,
                DebugSrc = debugSrc
            });
        }

        /// <summary>
        /// Append script element
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        /// <param name="isAsync">A value indicating whether to add an attribute "async" or not for js files</param>
        public virtual void AppendScriptParts(ResourceLocation location, string src, string debugSrc, bool excludeFromBundle, bool isAsync)
        {
            if (!_scriptParts.ContainsKey(location))
                _scriptParts.Add(location, new List<ScriptReferenceMeta>());

            if (string.IsNullOrEmpty(src))
                return;

            if (string.IsNullOrEmpty(debugSrc))
                debugSrc = src;

            _scriptParts[location].Insert(0, new ScriptReferenceMeta
            {
                ExcludeFromBundle = excludeFromBundle,
                IsAsync = isAsync,
                Src = src,
                DebugSrc = debugSrc
            });
        }

        /// <summary>
        /// Generate all script parts
        /// </summary>
        /// <param name="urlHelper">URL Helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="bundleFiles">A value indicating whether to bundle script elements</param>
        /// <returns>Generated string</returns>
        public virtual string GenerateScripts(IUrlHelper urlHelper, ResourceLocation location, bool bundleFiles)
        {
            if (!_scriptParts.ContainsKey(location) || _scriptParts[location] == null)
                return "";

            if (!_scriptParts.Any())
                return "";

            var debugModel = _hostingEnvironment.IsDevelopment();

            if (bundleFiles)
            {
                var partsToBundle = _scriptParts[location]
                    .Where(x => !x.ExcludeFromBundle)
                    .Distinct()
                    .ToArray();
                var partsToDontBundle = _scriptParts[location]
                    .Where(x => x.ExcludeFromBundle)
                    .Distinct()
                    .ToArray();

                var result = new StringBuilder();

                //parts to  bundle
                if (partsToBundle.Any())
                {
                    //ensure \bundles directory exists
                    _fileProvider.CreateDirectory(_fileProvider.GetAbsolutePath("bundles"));

                    var bundle = new Bundle();
                    foreach (var item in partsToBundle)
                    {
                        new PathString(urlHelper.Content(debugModel ? item.DebugSrc : item.Src))
                            .StartsWithSegments(urlHelper.ActionContext.HttpContext.Request.PathBase,
                                out PathString path);
                        var src = path.Value.TrimStart('/');

                        //check whether this file exists, if not it should be stored into /wwwroot directory
                        if (!_fileProvider.FileExists(_fileProvider.MapPath(path)))
                            src = $"wwwroot/{src}";

                        bundle.InputFiles.Add(src);
                    }
                    //output file
                    var outputFileName =
                        GetBundleFileName(partsToBundle.Select(x => debugModel ? x.DebugSrc : x.Src).ToArray());
                    bundle.OutputFileName = "wwwroot/bundles/" + outputFileName + ".js";
                    //save
                    var configFilePath = _hostingEnvironment.ContentRootPath + "\\" + outputFileName + ".json";
                    bundle.FileName = configFilePath;
                    lock (s_lock)
                    {
                        var cacheKey = $"minification.shouldrebuild.js-{outputFileName}";
                        var shouldRebuild = _staticCacheManager.Get(cacheKey, () => true, 120);
                        if (shouldRebuild)
                        {
                            //process
                            _processor.Process(configFilePath, new List<Bundle> {bundle});
                            _staticCacheManager.Set(cacheKey, false, 120);
                        }
                    }
                    //render
                    result.AppendFormat("<script src=\"{0}\"></script>",
                        urlHelper.Content("~/bundles/" + outputFileName + ".min.js"));
                    result.Append(Environment.NewLine);
                }

                //parts to not bundle
                foreach (var item in partsToDontBundle)
                {
                    var src = debugModel ? item.DebugSrc : item.Src;
                    result.AppendFormat("<script {1}src=\"{0}\"></script>", urlHelper.Content(src),
                        item.IsAsync ? "async " : "");
                    result.Append(Environment.NewLine);
                }
                return result.ToString();
            }
            else
            {
                //bundling is disabled
                var result = new StringBuilder();
                foreach (var item in _scriptParts[location].Distinct())
                {
                    var src = debugModel ? item.DebugSrc : item.Src;
                    result.AppendFormat("<script {1}src=\"{0}\"></script>", urlHelper.Content(src),
                        item.IsAsync ? "async " : "");
                    result.Append(Environment.NewLine);
                }
                return result.ToString();
            }
        }

        /// <summary>
        /// Add inline script element
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <param name="script">Script</param>
        public virtual void AddInlineScriptParts(ResourceLocation location, string script)
        {
            if (!_inlineScriptParts.ContainsKey(location))
                _inlineScriptParts.Add(location, new List<string>());

            if (string.IsNullOrEmpty(script))
                return;

            _inlineScriptParts[location].Add(script);
        }

        /// <summary>
        /// Append inline script element
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <param name="script">Script</param>
        public virtual void AppendInlineScriptParts(ResourceLocation location, string script)
        {
            if (!_inlineScriptParts.ContainsKey(location))
                _inlineScriptParts.Add(location, new List<string>());

            if (string.IsNullOrEmpty(script))
                return;

            _inlineScriptParts[location].Insert(0, script);
        }

        /// <summary>
        /// Generate all inline script parts
        /// </summary>
        /// <param name="urlHelper">URL Helper</param>
        /// <param name="location">A location of the script element</param>
        /// <returns>Generated string</returns>
        public virtual string GenerateInlineScripts(IUrlHelper urlHelper, ResourceLocation location)
        {
            if (!_inlineScriptParts.ContainsKey(location) || _inlineScriptParts[location] == null)
                return "";

            if (!_inlineScriptParts.Any())
                return "";

            var result = new StringBuilder();
            foreach (var item in _inlineScriptParts[location])
            {
                result.Append(item);
                result.Append(Environment.NewLine);
            }
            return result.ToString();
        }

        /// <summary>
        /// Add CSS element
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        public virtual void AddCssFileParts(ResourceLocation location, string src, string debugSrc,
            bool excludeFromBundle = false)
        {
            if (!_cssParts.ContainsKey(location))
                _cssParts.Add(location, new List<CssReferenceMeta>());

            if (string.IsNullOrEmpty(src))
                return;

            if (string.IsNullOrEmpty(debugSrc))
                debugSrc = src;

            _cssParts[location].Add(new CssReferenceMeta
            {
                ExcludeFromBundle = excludeFromBundle,
                Src = src,
                DebugSrc = debugSrc
            });
        }

        /// <summary>
        /// Append CSS element
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        public virtual void AppendCssFileParts(ResourceLocation location, string src, string debugSrc, bool excludeFromBundle = false)
        {
            if (!_cssParts.ContainsKey(location))
                _cssParts.Add(location, new List<CssReferenceMeta>());

            if (string.IsNullOrEmpty(src))
                return;

            if (string.IsNullOrEmpty(debugSrc))
                debugSrc = src;

            _cssParts[location].Insert(0, new CssReferenceMeta
            {
                ExcludeFromBundle = excludeFromBundle,
                Src = src,
                DebugSrc = debugSrc
            });
        }

        /// <summary>
        /// Generate all CSS parts
        /// </summary>
        /// <param name="urlHelper">URL Helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="bundleFiles">A value indicating whether to bundle script elements</param>
        /// <returns>Generated string</returns>
        public virtual string GenerateCssFiles(IUrlHelper urlHelper, ResourceLocation location, bool bundleFiles)
        {
            if (!_cssParts.ContainsKey(location) || _cssParts[location] == null)
                return "";

            if (!_cssParts.Any())
                return "";

            var debugModel = _hostingEnvironment.IsDevelopment();

            //CSS bundling is not allowed in virtual directories
            if (urlHelper.ActionContext.HttpContext.Request.PathBase.HasValue)
                bundleFiles = false;

            if (bundleFiles)
            {
                var partsToBundle = _cssParts[location]
                    .Where(x => !x.ExcludeFromBundle)
                    .Distinct()
                    .ToArray();
                var partsToDontBundle = _cssParts[location]
                    .Where(x => x.ExcludeFromBundle)
                    .Distinct()
                    .ToArray();

                var result = new StringBuilder();

                //parts to  bundle
                if (partsToBundle.Any())
                {
                    //ensure \bundles directory exists
                    _fileProvider.CreateDirectory(_fileProvider.GetAbsolutePath("bundles"));

                    var bundle = new Bundle();
                    foreach (var item in partsToBundle)
                    {
                        var src = debugModel ? item.DebugSrc : item.Src;
                        src = urlHelper.Content(src);
                        //check whether this file exists 
                        var srcPath = _fileProvider.Combine(_hostingEnvironment.ContentRootPath,
                            src.Remove(0, 1).Replace("/", "\\"));
                        if (_fileProvider.FileExists(srcPath))
                        {
                            //remove starting /
                            src = src.Remove(0, 1);
                        }
                        else
                        {
                            //if not, it should be stored into /wwwroot directory
                            src = "wwwroot/" + src;
                        }
                        bundle.InputFiles.Add(src);
                    }
                    //output file
                    var outputFileName = GetBundleFileName(partsToBundle.Select(x =>
                    {
                        return debugModel ? x.DebugSrc : x.Src;
                    }).ToArray());
                    bundle.OutputFileName = "wwwroot/bundles/" + outputFileName + ".css";
                    //save
                    var configFilePath = _hostingEnvironment.ContentRootPath + "\\" + outputFileName + ".json";
                    bundle.FileName = configFilePath;
                    lock (s_lock)
                    {
                        var cacheKey = $"minification.shouldrebuild.css-{outputFileName}";
                        var shouldRebuild = _staticCacheManager.Get(cacheKey, () => true, 120);
                        if (shouldRebuild)
                        {
                            //process
                            _processor.Process(configFilePath, new List<Bundle> {bundle});
                            _staticCacheManager.Set(cacheKey, false, 120);
                        }
                    }

                    //render
                    result.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"{1}\" />",
                        urlHelper.Content("~/bundles/" + outputFileName + ".min.css"), "text/css");
                    result.Append(Environment.NewLine);
                }

                //parts not to bundle
                foreach (var item in partsToDontBundle)
                {
                    var src = debugModel ? item.DebugSrc : item.Src;
                    result.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"{1}\" />",
                        urlHelper.Content(src),
                        "text/css");
                    result.Append(Environment.NewLine);
                }

                return result.ToString();
            }
            else
            {
                //bundling is disabled
                var result = new StringBuilder();
                foreach (var item in _cssParts[location].Distinct())
                {
                    var src = debugModel ? item.DebugSrc : item.Src;
                    result.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"{1}\" />",
                        urlHelper.Content(src),
                        "text/css");
                    result.AppendLine();
                }
                return result.ToString();
            }
        }

        /// <summary>
        /// Add CSS class to the <![CDATA[<head>]]> element
        /// </summary>
        /// <param name="part">CSS class</param>
        public virtual void AddPageCssClassParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _pageCssClassParts.Add(part);
        }

        /// <summary>
        /// Append CSS class to the <![CDATA[<head>]]> element
        /// </summary>
        /// <param name="part">CSS class</param>
        public virtual void AppendPageCssClassParts(string part)
        {
            if (string.IsNullOrEmpty(part))
                return;

            _pageCssClassParts.Insert(0, part);
        }

        /// <summary>
        /// Specify "edit page" URL
        /// </summary>
        /// <param name="url">URL</param>
        public virtual void AddEditPageUrl(string url)
        {
            _editPageUrl = url;
        }

        /// <summary>
        /// Get "edit page" URL
        /// </summary>
        /// <returns>URL</returns>
        public virtual string GetEditPageUrl()
        {
            return _editPageUrl;
        }

        /// <summary>
        /// Specify system name of admin menu item that should be selected (expanded)
        /// </summary>
        /// <param name="systemName">System name</param>
        public virtual void SetActiveMenuItemSystemName(string systemName)
        {
            _activeAdminMenuSystemName = systemName;
        }

        /// <summary>
        /// Get system name of admin menu item that should be selected (expanded)
        /// </summary>
        /// <returns>System name</returns>
        public virtual string GetActiveMenuItemSystemName()
        {
            return _activeAdminMenuSystemName;
        }

        #region Nested classes

        /// <summary>
        /// JS file meta data
        /// </summary>
        private class ScriptReferenceMeta : IEquatable<ScriptReferenceMeta>
        {
            /// <summary>
            /// A value indicating whether to exclude the script from bundling
            /// </summary>
            public bool ExcludeFromBundle { get; set; }

            /// <summary>
            /// A value indicating whether to load the script asynchronously 
            /// </summary>
            public bool IsAsync { get; set; }

            /// <summary>
            /// Src for production
            /// </summary>
            public string Src { get; set; }

            /// <summary>
            /// Src for debugging
            /// </summary>
            public string DebugSrc { get; set; }

            /// <summary>
            /// Equals
            /// </summary>
            /// <param name="item">Other item</param>
            /// <returns>Result</returns>
            public bool Equals(ScriptReferenceMeta item)
            {
                if (item == null)
                    return false;
                return this.Src.Equals(item.Src) && this.DebugSrc.Equals(item.DebugSrc);
            }

            /// <summary>
            /// Get hash code
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return Src == null ? 0 : Src.GetHashCode();
            }
        }

        /// <summary>
        /// CSS file meta data
        /// </summary>
        private class CssReferenceMeta : IEquatable<CssReferenceMeta>
        {
            public bool ExcludeFromBundle { get; set; }

            /// <summary>
            /// Src for production
            /// </summary>
            public string Src { get; set; }

            /// <summary>
            /// Src for debugging
            /// </summary>
            public string DebugSrc { get; set; }

            /// <summary>
            /// Equals
            /// </summary>
            /// <param name="item">Other item</param>
            /// <returns>Result</returns>
            public bool Equals(CssReferenceMeta item)
            {
                if (item == null)
                    return false;
                return this.Src.Equals(item.Src) && this.DebugSrc.Equals(item.DebugSrc);
            }

            /// <summary>
            /// Get hash code
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return Src == null ? 0 : Src.GetHashCode();
            }
        }

        #endregion
    }
}