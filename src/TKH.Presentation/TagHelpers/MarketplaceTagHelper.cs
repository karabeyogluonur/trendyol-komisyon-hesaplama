using Microsoft.AspNetCore.Razor.TagHelpers;
using TKH.Entities.Enums;
using TKH.Presentation.Extensions;

namespace TKH.Presentation.TagHelpers
{
    [HtmlTargetElement("marketplace-logo")]
    public class MarketplaceLogoTagHelper : TagHelper
    {
        public MarketplaceType Type { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";
            output.TagMode = TagMode.SelfClosing;

            var src = Type.GetLogoUrl();
            var alt = Type.ToString();

            output.Attributes.SetAttribute("src", src);
            output.Attributes.SetAttribute("alt", alt);
        }
    }
}
