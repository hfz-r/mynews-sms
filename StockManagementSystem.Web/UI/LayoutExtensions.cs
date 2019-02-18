using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockManagementSystem.Core.Infrastructure;

namespace StockManagementSystem.Web.UI
{
    /// <summary>
    /// Layout extensions
    /// </summary>
    public static class LayoutExtensions
    {
        /// <summary>
        /// Add title element to the <![CDATA[<head>]]>
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">Title part</param>
        public static void AddTitleParts(this IHtmlHelper html, string part)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddTitleParts(part);
        }

        /// <summary>
        /// Add script element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        /// <param name="isAsync">A value indicating whether to add an attribute "async" or not for js files</param>
        public static void AddScriptParts(this IHtmlHelper html, string src, string debugSrc = "",
            bool excludeFromBundle = false, bool isAsync = false)
        {
            AddScriptParts(html, ResourceLocation.Head, src, debugSrc, excludeFromBundle, isAsync);
        }

        /// <summary>
        /// Add script element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        /// <param name="isAsync">A value indicating whether to add an attribute "async" or not for js files</param>
        public static void AddScriptParts(this IHtmlHelper html, ResourceLocation location,
            string src, string debugSrc = "", bool excludeFromBundle = false, bool isAsync = false)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddScriptParts(location, src, debugSrc, excludeFromBundle, isAsync);
        }

        /// <summary>
        /// Append script element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        /// <param name="isAsync">A value indicating whether to add an attribute "async" or not for js files</param>
        public static void AppendScriptParts(this IHtmlHelper html, string src, string debugSrc = "", bool excludeFromBundle = false, bool isAsync = false)
        {
            AppendScriptParts(html, ResourceLocation.Head, src, debugSrc, excludeFromBundle, isAsync);
        }

        /// <summary>
        /// Append script element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        /// <param name="isAsync">A value indicating whether to add an attribute "async" or not for js files</param>
        public static void AppendScriptParts(this IHtmlHelper html, ResourceLocation location,
            string src, string debugSrc = "", bool excludeFromBundle = false, bool isAsync = false)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendScriptParts(location, src, debugSrc, excludeFromBundle, isAsync);
        }

        /// <summary>
        /// Generate all script parts
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="urlHelper">URL Helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="bundleFiles">A value indicating whether to bundle script elements</param>
        /// <returns>Generated string</returns>
        public static IHtmlContent DefaultScripts(this IHtmlHelper html, IUrlHelper urlHelper, ResourceLocation location, bool bundleFiles)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            return new HtmlString(pageHeadBuilder.GenerateScripts(urlHelper, location, bundleFiles));
        }

        /// <summary>
        /// Add inline script element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="script">Script</param>
        public static void AddInlineScriptParts(this IHtmlHelper html, ResourceLocation location, string script)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddInlineScriptParts(location, script);
        }

        /// <summary>
        /// Append inline script element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="script">Script</param>
        public static void AppendInlineScriptParts(this IHtmlHelper html, ResourceLocation location, string script)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendInlineScriptParts(location, script);
        }

        /// <summary>
        /// Generate all inline script parts
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="urlHelper">URL Helper</param>
        /// <param name="location">A location of the script element</param>
        /// <returns>Generated string</returns>
        public static IHtmlContent DefaultInlineScripts(this IHtmlHelper html, IUrlHelper urlHelper, ResourceLocation location)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            return new HtmlString(pageHeadBuilder.GenerateInlineScripts(urlHelper, location));
        }

        /// <summary>
        /// Add CSS element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        public static void AddCssFileParts(this IHtmlHelper html, string src, string debugSrc = "",
            bool excludeFromBundle = false)
        {
            AddCssFileParts(html, ResourceLocation.Head, src, debugSrc, excludeFromBundle);
        }

        /// <summary>
        /// Add CSS element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        public static void AddCssFileParts(this IHtmlHelper html, ResourceLocation location,
            string src, string debugSrc = "", bool excludeFromBundle = false)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddCssFileParts(location, src, debugSrc, excludeFromBundle);
        }

        /// <summary>
        /// Append CSS element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        public static void AppendCssFileParts(this IHtmlHelper html, string src, string debugSrc = "", bool excludeFromBundle = false)
        {
            AppendCssFileParts(html, ResourceLocation.Head, src, debugSrc, excludeFromBundle);
        }

        /// <summary>
        /// Append CSS element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        public static void AppendCssFileParts(this IHtmlHelper html, ResourceLocation location,
            string src, string debugSrc = "", bool excludeFromBundle = false)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendCssFileParts(location, src, debugSrc, excludeFromBundle);
        }

        /// <summary>
        /// Generate all CSS parts
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="urlHelper">URL Helper</param>
        /// <param name="location">A location of the script element</param>
        /// <param name="bundleFiles">A value indicating whether to bundle script elements</param>
        /// <returns>Generated string</returns>
        public static IHtmlContent DefaultCssFiles(this IHtmlHelper html, IUrlHelper urlHelper, ResourceLocation location, bool bundleFiles)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            return new HtmlString(pageHeadBuilder.GenerateCssFiles(urlHelper, location, bundleFiles));
        }

        /// <summary>
        /// Add CSS class to the <![CDATA[<head>]]> element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">CSS class</param>
        public static void AddPageCssClassParts(this IHtmlHelper html, string part)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AddPageCssClassParts(part);
        }

        /// <summary>
        /// Append CSS class to the <![CDATA[<head>]]> element
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="part">CSS class</param>
        public static void AppendPageCssClassParts(this IHtmlHelper html, string part)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.AppendPageCssClassParts(part);
        }

        /// <summary>
        /// Specify system name of admin menu item that should be selected (expanded)
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <param name="systemName">System name</param>
        public static void SetActiveMenuItemSystemName(this IHtmlHelper html, string systemName)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            pageHeadBuilder.SetActiveMenuItemSystemName(systemName);
        }

        /// <summary>
        /// Get system name of admin menu item that should be selected (expanded)
        /// </summary>
        /// <param name="html">HTML helper</param>
        /// <returns>System name</returns>
        public static string GetActiveMenuItemSystemName(this IHtmlHelper html)
        {
            var pageHeadBuilder = EngineContext.Current.Resolve<IPageHeadBuilder>();
            return pageHeadBuilder.GetActiveMenuItemSystemName();
        }
    }
}