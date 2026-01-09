using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace TKH.Web.Infrastructure.TagHelpers
{
    [HtmlTargetElement("app-submit-button")]
    public class SubmitButtonTagHelper : TagHelper
    {
        public string Text { get; set; } = "Kaydet";

        public string LoadingText { get; set; } = "Lütfen bekleyin...";

        public string Id { get; set; } = "kt_btn_submit";

        public string Icon { get; set; } = string.Empty;

        public override void Process(TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput)
        {
            tagHelperOutput.TagName = "button";
            tagHelperOutput.TagMode = TagMode.StartTagAndEndTag;

            string existingClass = tagHelperOutput.Attributes["class"]?.Value?.ToString() ?? string.Empty;
            string cssClass = string.IsNullOrEmpty(existingClass) ? "btn btn-sm btn-primary" : existingClass;
            tagHelperOutput.Attributes.SetAttribute("class", cssClass);

            tagHelperOutput.Attributes.SetAttribute("type", "submit");
            tagHelperOutput.Attributes.SetAttribute("id", Id);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<span class=\"indicator-label\">");

            if (!string.IsNullOrEmpty(Icon))
                stringBuilder.Append($"<i class=\"ki-outline ki-{Icon} fs-4 me-1\"></i>");

            stringBuilder.Append(Text);
            stringBuilder.Append("</span>");

            stringBuilder.Append("<span class=\"indicator-progress\">");
            stringBuilder.Append(LoadingText);
            stringBuilder.Append("<span class=\"spinner-border spinner-border-sm align-middle ms-2\"></span>");
            stringBuilder.Append("</span>");

            tagHelperOutput.Content.SetHtmlContent(stringBuilder.ToString());
        }
    }
}
