﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StockManagementSystem.Core;
using StockManagementSystem.Web.Extensions;

namespace StockManagementSystem.Web.TagHelpers
{
    [HtmlTargetElement("_editor", Attributes = ForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class EditorTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const string DisabledAttributeName = "asp-disabled";
        private const string ReadOnlyAttributeName = "asp-readonly";
        private const string RequiredAttributeName = "asp-required";
        private const string RenderFormControlClassAttributeName = "asp-render-form-control-class";
        private const string TemplateAttributeName = "asp-template";
        private const string PostfixAttributeName = "asp-postfix";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// Indicates whether the field is disabled
        /// </summary>
        [HtmlAttributeName(DisabledAttributeName)]
        public string IsDisabled { set; get; }

        /// <summary>
        /// Indicates whether the field is readonly
        /// </summary>
        [HtmlAttributeName(ReadOnlyAttributeName)]
        public string IsReadOnly { get; set; }

        /// <summary>
        /// Indicates whether the field is required
        /// </summary>
        [HtmlAttributeName(RequiredAttributeName)]
        public string IsRequired { set; get; }

        /// <summary>
        /// Indicates whether the "form-control" class should be added to the input
        /// </summary>
        [HtmlAttributeName(RenderFormControlClassAttributeName)]
        public string RenderFormControlClass { set; get; }

        /// <summary>
        /// Editor template for the field
        /// </summary>
        [HtmlAttributeName(TemplateAttributeName)]
        public string Template { set; get; }

        /// <summary>
        /// Postfix
        /// </summary>
        [HtmlAttributeName(PostfixAttributeName)]
        public string Postfix { set; get; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public EditorTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            //clear the output
            output.SuppressOutput();

            //container for additional attributes
            var htmlAttributes = new Dictionary<string, object>();

            //disabled attribute
            bool.TryParse(IsDisabled, out bool disabled);
            if (disabled)
                htmlAttributes.Add("disabled", "disabled");

            //readonly attribute
            bool.TryParse(IsReadOnly, out bool @readonly);
            if (@readonly)
                htmlAttributes.Add("readonly", "readonly");

            //required asterisk
            bool.TryParse(IsRequired, out bool required);
            if (required)
            {
                output.PreElement.SetHtmlContent("<div class='input-group input-group-required'>");
                output.PostElement.SetHtmlContent("<div class=\"input-group-btn\"><span class=\"required\">*</span></div></div>");
            }

            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //add form-control class
            bool.TryParse(RenderFormControlClass, out bool renderFormControlClass);
            if (string.IsNullOrEmpty(RenderFormControlClass) && For.Metadata.ModelType.Name.Equals("String") || renderFormControlClass)
                htmlAttributes.Add("class", "form-control");

            var viewEngine = CommonHelper.GetPrivateFieldValue(_htmlHelper, "_viewEngine") as IViewEngine;
            var bufferScope = CommonHelper.GetPrivateFieldValue(_htmlHelper, "_bufferScope") as IViewBufferScope;
            var templateBuilder = new TemplateBuilder(
                viewEngine,
                bufferScope,
                _htmlHelper.ViewContext,
                _htmlHelper.ViewData,
                For.ModelExplorer,
                For.Name,
                Template,
                readOnly: false,
                additionalViewData: new {htmlAttributes, postfix = this.Postfix});

            var htmlOutput = templateBuilder.Build();
            output.Content.SetHtmlContent(htmlOutput.RenderHtmlContent());
        }
    }
}