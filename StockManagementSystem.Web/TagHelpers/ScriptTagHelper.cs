using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StockManagementSystem.Web.Extensions;
using StockManagementSystem.Web.UI;

namespace StockManagementSystem.Web.TagHelpers
{
    [HtmlTargetElement("_script", Attributes = LocationAttributeName)]
    public class ScriptTagHelper : TagHelper
    {
        private const string LocationAttributeName = "asp-location";
        private readonly IHtmlHelper _htmlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Indicates where the script should be rendered
        /// </summary>
        [HtmlAttributeName(LocationAttributeName)]
        public ResourceLocation Location { set; get; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public ScriptTagHelper(IHtmlHelper htmlHelper, IHttpContextAccessor httpContextAccessor)
        {
            this._htmlHelper = htmlHelper;
            this._httpContextAccessor = httpContextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (_httpContextAccessor.HttpContext.Items["IgnoreScriptTagLocation"] != null &&
                Convert.ToBoolean(_httpContextAccessor.HttpContext.Items["IgnoreScriptTagLocation"]))
                return;

            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //get JavaScript
            var script = output.GetChildContentAsync().Result.GetContent();

            //build script tag
            var scriptTag = new TagBuilder("script");
            scriptTag.InnerHtml.SetHtmlContent(new HtmlString(script));

            //merge attributes
            foreach (var attribute in context.AllAttributes)
                if (!attribute.Name.StartsWith("asp-"))
                    scriptTag.Attributes.Add(attribute.Name, attribute.Value.ToString());

            _htmlHelper.AddInlineScriptParts(Location, scriptTag.RenderHtmlContent());

            output.SuppressOutput();
        }
    }

}