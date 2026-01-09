using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TKH.Web.Infrastructure.TagHelpers
{
    public enum AlertType
    {
        Info,
        Warning,
        Success,
        Danger,
        Primary,
        Secondary,
        Dark,
        Error
    }

    [HtmlTargetElement("app-alert")]
    public class AlertTagHelper : TagHelper
    {
        public AlertType Type { get; set; } = AlertType.Info;
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public bool Dismissible { get; set; }

        public override async Task ProcessAsync(TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput)
        {
            string colorClass = Type switch
            {
                AlertType.Success => "success",
                AlertType.Warning => "warning",
                AlertType.Danger => "danger",
                AlertType.Error => "danger",
                AlertType.Primary => "primary",
                AlertType.Secondary => "secondary",
                AlertType.Dark => "dark",
                _ => "info"
            };

            string iconClass = !string.IsNullOrEmpty(Icon) ? Icon : Type switch
            {
                AlertType.Success => "ki-check-circle",
                AlertType.Warning => "ki-information-5",
                AlertType.Danger => "ki-shield-cross",
                AlertType.Error => "ki-cross-circle",
                AlertType.Primary => "ki-abstract-26",
                AlertType.Secondary => "ki-dots-square",
                AlertType.Dark => "ki-moon",
                _ => "ki-information-2"
            };

            tagHelperOutput.TagName = "div";

            string alignmentClass = string.IsNullOrEmpty(Title) ? "align-items-center" : "align-items-start";
            string dismissibleClass = Dismissible ? "alert-dismissible" : string.Empty;

            tagHelperOutput.Attributes.SetAttribute("class", $"alert {dismissibleClass} bg-light-{colorClass} border border-{colorClass} border-dashed d-flex flex-column flex-sm-row {alignmentClass} p-4 mb-4");

            if (Dismissible)
            {
                tagHelperOutput.Content.AppendHtml(
                    $"<button type='button' " +
                    $"class='btn btn-icon btn-sm btn-active-light-{colorClass} position-absolute top-0 end-0 mt-1 me-1' " +
                    $"data-bs-dismiss='alert'>" +
                    $"<i class='ki-outline ki-cross fs-4 text-{colorClass}'></i>" +
                    $"</button>"
                );
            }

            tagHelperOutput.Content.AppendHtml($"<i class='ki-outline {iconClass} fs-2hx text-{colorClass} me-sm-4 mb-2 mb-sm-0 flex-shrink-0'></i>");

            tagHelperOutput.Content.AppendHtml("<div class='d-flex flex-column w-100'>");

            if (!string.IsNullOrEmpty(Title))
            {
                tagHelperOutput.Content.AppendHtml($"<div class='fw-bold fs-7 text-{colorClass} mb-1'>{Title}</div>");
            }

            string fontSizeClass = string.IsNullOrEmpty(Title) ? "fs-7 fw-semibold" : "fs-8";
            tagHelperOutput.Content.AppendHtml($"<div class='{fontSizeClass} text-gray-700 lh-sm'>");

            TagHelperContent childContent = await tagHelperOutput.GetChildContentAsync();
            tagHelperOutput.Content.AppendHtml(childContent);

            tagHelperOutput.Content.AppendHtml("</div></div>");
        }
    }
}
