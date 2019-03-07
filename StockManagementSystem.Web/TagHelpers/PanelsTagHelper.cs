using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace StockManagementSystem.Web.TagHelpers
{
    [HtmlTargetElement("my-panels", Attributes = IdAttributeName)]
    public class PanelsTagHelper : TagHelper
    {
        private const string IdAttributeName = "id";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }
    }
}