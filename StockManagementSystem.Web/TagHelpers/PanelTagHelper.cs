using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StockManagementSystem.Web.Extensions;

namespace StockManagementSystem.Web.TagHelpers
{
    [HtmlTargetElement("my-panel", ParentTag = "my-panels", Attributes = NameAttributeName)]
    public class PanelTagHelper : TagHelper
    {
        private const string NameAttributeName = "asp-name";
        private const string TitleAttributeName = "asp-title";
        private const string HideBlockAttributeName = "asp-hide-block-attribute-name";
        private const string IsHideAttributeName= "asp-hide";
        private const string IsAdvancedAttributeName = "asp-advanced";
        private const string PanelIconAttributeName = "asp-icon";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// Title of the panel
        /// </summary>
        [HtmlAttributeName(TitleAttributeName)]
        public string Title { get; set; }

        /// <summary>
        /// Name of the panel
        /// </summary>
        [HtmlAttributeName(NameAttributeName)]
        public string Name { get; set; }

        /// <summary>
        /// Name of the hide attribute of the panel
        /// </summary>
        [HtmlAttributeName(HideBlockAttributeName)]
        public string HideBlock { get; set; }

        /// <summary>
        /// Indicates whether a block is hidden or not
        /// </summary>
        [HtmlAttributeName(IsHideAttributeName)]
        public bool IsHide { get; set; }

        /// <summary>
        /// Indicates whether a panel is advanced or not
        /// </summary>
        [HtmlAttributeName(IsAdvancedAttributeName)]
        public bool IsAdvanced { get; set; }

        /// <summary>
        /// Panel icon
        /// </summary>
        [HtmlAttributeName(PanelIconAttributeName)]
        public string PanelIconIsAdvanced { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public PanelTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //create panel
            var panel = new TagBuilder("div");
            panel.AddCssClass("panel panel-default collapsible-panel");
            if (context.AllAttributes.ContainsName(IsAdvancedAttributeName) && context.AllAttributes[IsAdvancedAttributeName].Value.Equals(true))
            {
                panel.AddCssClass("advanced-setting");
            }

            //create panel heading and append title and icon to it
            var panelHeading = new TagBuilder("div");
            panelHeading.AddCssClass("panel-heading");
            panelHeading.Attributes.Add("data-hideAttribute", context.AllAttributes[HideBlockAttributeName].Value.ToString());

            if (context.AllAttributes[IsHideAttributeName].Value.Equals(false))
            {
                panelHeading.AddCssClass("opened");
            }

            if (context.AllAttributes.ContainsName(PanelIconAttributeName))
            {
                var panelIcon = new TagBuilder("i");
                panelIcon.AddCssClass("panel-icon");
                panelIcon.AddCssClass(context.AllAttributes[PanelIconAttributeName].Value.ToString());
                var iconContainer = new TagBuilder("div");
                iconContainer.AddCssClass("icon-container");
                iconContainer.InnerHtml.AppendHtml(panelIcon);
                panelHeading.InnerHtml.AppendHtml(iconContainer);
            }

            panelHeading.InnerHtml.AppendHtml($"<span>{context.AllAttributes[TitleAttributeName].Value}</span>");

            var collapseIcon = new TagBuilder("i");
            collapseIcon.AddCssClass("fa");
            collapseIcon.AddCssClass("toggle-icon");
            collapseIcon.AddCssClass(context.AllAttributes[IsHideAttributeName].Value.Equals(true) ? "fa-plus" : "fa-minus");
            panelHeading.InnerHtml.AppendHtml(collapseIcon);

            //create inner panel container to toggle on click and add data to it
            var panelContainer = new TagBuilder("div");
            panelContainer.AddCssClass("panel-container");
            if (context.AllAttributes[IsHideAttributeName].Value.Equals(true))
            {
                panelContainer.AddCssClass("collapsed");
            }

            panelContainer.InnerHtml.AppendHtml(output.GetChildContentAsync().Result.GetContent());

            //add heading and container to panel
            panel.InnerHtml.AppendHtml(panelHeading);
            panel.InnerHtml.AppendHtml(panelContainer);

            output.Content.AppendHtml(panel.RenderHtmlContent());
        }
    }
}