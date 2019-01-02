using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StockManagementSystem.Web.Extensions;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Web.TagHelpers
{
    [HtmlTargetElement("_alert", Attributes = AlertNameId, TagStructure = TagStructure.WithoutEndTag)]
    public class AlertTagHelper : TagHelper
    {
        private const string AlertNameId = "asp-alert-id";
        private const string AlertMessageName = "asp-alert-message";

        private readonly IHtmlHelper _htmlHelper;

        protected IHtmlGenerator Generator { get; set; }

        /// <summary>
        /// Alert identifier
        /// </summary>
        [HtmlAttributeName(AlertNameId)]
        public string AlertId { get; set; }

        /// <summary>
        /// Additional confirm text
        /// </summary>
        [HtmlAttributeName(AlertMessageName)]
        public string Message { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public AlertTagHelper(IHtmlHelper htmlHelper, IHtmlGenerator generator)
        {
            _htmlHelper = htmlHelper;
            Generator = generator;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            var modalId = new HtmlString(AlertId + "-action-alert").ToHtmlString();

            var actionAlertModel = new ActionAlertModel
            {
                AlertId = AlertId,
                WindowId = modalId,
                AlertMessage = Message
            };

            //tag details
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.Add("id", modalId);
            output.Attributes.Add("class", "modal fade");
            output.Attributes.Add("tabindex", "-1");
            output.Attributes.Add("role", "dialog");
            output.Attributes.Add("aria-labelledby", $"{modalId}-title");
            output.Content.SetHtmlContent(await _htmlHelper.PartialAsync("Alert", actionAlertModel));

            //modal script
            var script = new TagBuilder("script");
            script.InnerHtml.AppendHtml("$(document).ready(function () {" +
                                        $"$('#{AlertId}').attr(\"data-toggle\", \"modal\").attr(\"data-target\", \"#{modalId}\")" + "});");

            output.PostContent.SetHtmlContent(script.RenderHtmlContent());

        }
    }
}