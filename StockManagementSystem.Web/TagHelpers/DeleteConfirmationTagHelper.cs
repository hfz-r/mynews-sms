﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StockManagementSystem.Web.Extensions;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Web.TagHelpers
{
    [HtmlTargetElement("delete-confirmation", Attributes = ModelIdAttributeName + "," + ButtonIdAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class DeleteConfirmationTagHelper : TagHelper
    {
        private const string ModelIdAttributeName = "asp-model-id";
        private const string ButtonIdAttributeName = "asp-button-id";
        private const string ActionAttributeName = "asp-action";

        private readonly IHtmlHelper _htmlHelper;
        protected IHtmlGenerator Generator { get; set; }

        /// <summary>
        /// Model identifier
        /// </summary>
        [HtmlAttributeName(ModelIdAttributeName)]
        public string ModelId { get; set; }

        /// <summary>
        /// Button identifier
        /// </summary>
        [HtmlAttributeName(ButtonIdAttributeName)]
        public string ButtonId { get; set; }

        /// <summary>
        /// Delete action name
        /// </summary>
        [HtmlAttributeName(ActionAttributeName)]
        public string Action { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public DeleteConfirmationTagHelper(IHtmlHelper htmlHelper, IHtmlGenerator generator)
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

            if (string.IsNullOrEmpty(Action))
                Action = "Delete";

            var modelName = _htmlHelper.ViewData.ModelMetadata.ModelType.Name.ToLower();
            if (!string.IsNullOrEmpty(Action))
                modelName += "-" + Action;
            var modalId = new HtmlString(modelName + "-delete-confirmation").ToHtmlString();

            if (int.TryParse(ModelId, out int modelId))
            {
                var deleteConfirmationModel = new DeleteConfirmationModel
                {
                    Id = modelId,
                    ControllerName = _htmlHelper.ViewContext.RouteData.Values["controller"].ToString(),
                    ActionName = Action,
                    WindowId = modalId
                };

                //tag details
                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;

                output.Attributes.Add("id", modalId);
                output.Attributes.Add("class", "modal fade");
                output.Attributes.Add("tabindex", "-1");
                output.Attributes.Add("role", "dialog");
                output.Attributes.Add("aria-labelledby", $"{modalId}-title");
                output.Content.SetHtmlContent(await _htmlHelper.PartialAsync("Delete", deleteConfirmationModel));

                //modal script
                var script = new TagBuilder("script");
                script.InnerHtml.AppendHtml("$(document).ready(function () {" +
                                            $"$('#{ButtonId}').attr(\"data-toggle\", \"modal\").attr(\"data-target\", \"#{modalId}\")" + "});");
                output.PostContent.SetHtmlContent(script.RenderHtmlContent());
            }
        }
    }
}