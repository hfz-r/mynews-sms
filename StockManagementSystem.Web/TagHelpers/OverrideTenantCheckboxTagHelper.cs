﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StockManagementSystem.Web.Extensions;

namespace StockManagementSystem.Web.TagHelpers
{
    /// <summary>
    /// override-tenant-checkbox tag helper
    /// </summary>
    [HtmlTargetElement("_override-checkbox", Attributes = ForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class OverrideTenantCheckboxTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";
        private const string InputAttributeName = "asp-input";
        private const string Input2AttributeName = "asp-input2";
        private const string TenantScopeAttributeName = "asp-tenant-scope";
        private const string ParentContainerAttributeName = "asp-parent-container";

        private readonly IHtmlHelper _htmlHelper;

        /// <summary>
        /// An expression to be evaluated against the current model
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        /// <summary>
        /// The input #1
        /// </summary>
        [HtmlAttributeName(InputAttributeName)]
        public ModelExpression Input { set; get; }

        /// <summary>
        /// The input #2
        /// </summary>
        [HtmlAttributeName(Input2AttributeName)]
        public ModelExpression Input2 { set; get; }

        /// <summary>
        ///The tenant scope
        /// </summary>
        [HtmlAttributeName(TenantScopeAttributeName)]
        public int TenantScope { set; get; }

        /// <summary>
        /// Parent container
        /// </summary>
        [HtmlAttributeName(ParentContainerAttributeName)]
        public string ParentContainer { set; get; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public OverrideTenantCheckboxTagHelper(IHtmlHelper htmlHelper)
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

            //render only when a certain tenant is chosen
            if (TenantScope > 0)
            {
                //contextualize IHtmlHelper
                var viewContextAware = _htmlHelper as IViewContextAware;
                viewContextAware?.Contextualize(ViewContext);

                var dataInputIds = new List<string>();
                if (Input != null)
                    dataInputIds.Add(_htmlHelper.Id(Input.Name));
                if (Input2 != null)
                    dataInputIds.Add(_htmlHelper.Id(Input2.Name));

                const string cssClass = "multi-tenant-override-option";

                var dataInputSelector = "";
                if (!string.IsNullOrEmpty(ParentContainer))
                    dataInputSelector = "#" + ParentContainer + " input, #" + ParentContainer + " textarea, #" + ParentContainer + " select";
                if (dataInputIds.Any())
                    dataInputSelector = "#" + string.Join(", #", dataInputIds);

                var onClick = $"checkOverriddenTenantValue(this, '{dataInputSelector}')";

                var htmlAttributes = new
                {
                    @class = cssClass,
                    onclick = onClick,
                    data_for_input_selector = dataInputSelector
                };
                var htmlOutput = _htmlHelper.CheckBox(For.Name, null, htmlAttributes);
                output.Content.SetHtmlContent(htmlOutput.RenderHtmlContent());
            }
        }
    }
}