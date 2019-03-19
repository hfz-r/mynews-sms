using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Web.Extensions;

namespace StockManagementSystem.Web.TagHelpers
{
    [HtmlTargetElement("my-nested-setting", Attributes = ForAttributeName)]
    public class NestedSettingTagHelper : TagHelper
    {
        private readonly CommonSettings _commonSettings;

        private const string ForAttributeName = "asp-for";

        /// <summary>
        /// HtmlGenerator
        /// </summary>
        protected IHtmlGenerator Generator { get; set; }

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public NestedSettingTagHelper(IHtmlGenerator generator, CommonSettings commonSettings)
        {
            Generator = generator;
            _commonSettings = commonSettings;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            var parentSettingName = For.Name;

            var random = CommonHelper.GenerateRandomInteger();
            var nestedSettingId = $"nestedSetting{random}";
            var parentSettingId = $"parentSetting{random}";

            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("class", "nested-setting");
            if (context.AllAttributes.ContainsName("id"))
                nestedSettingId = context.AllAttributes["id"].Value.ToString();

            if (_commonSettings.UseNestedSetting)
            {
                var script = new TagBuilder("script");
                script.InnerHtml.AppendHtml("$(document).ready(function () {" +
                                                $"initNestedSetting('{parentSettingName}', '{parentSettingId}', '{nestedSettingId}');" +
                                            "});");
                output.PreContent.SetHtmlContent(script.RenderHtmlContent());
            }
        }
    }
}