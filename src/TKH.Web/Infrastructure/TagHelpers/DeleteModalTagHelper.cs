using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace TKH.Web.Infrastructure.TagHelpers
{
    [HtmlTargetElement("app-delete-modal")]
    public class DeleteModalTagHelper : TagHelper
    {
        private readonly IHtmlGenerator _htmlGenerator;

        public DeleteModalTagHelper(IHtmlGenerator htmlGenerator)
        {
            _htmlGenerator = htmlGenerator;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public string Id { get; set; } = "deleteModal";
        public string Title { get; set; } = "Silme Onayı";
        public string Description { get; set; } = "Bu işlem geri alınamaz. Devam etmek istiyor musunuz?";
        public string Controller { get; set; }
        public string Action { get; set; } = "Delete";
        public string InputId { get; set; } = "deleteIdInput";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "modal fade");
            output.Attributes.SetAttribute("id", Id);
            output.Attributes.SetAttribute("tabindex", "-1");
            output.Attributes.SetAttribute("aria-hidden", "true");

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<div class='modal-dialog modal-dialog-centered'>");
            stringBuilder.Append("<div class='modal-content'>");

            stringBuilder.Append("<div class='modal-header'>");
            stringBuilder.Append($"<h5 class='modal-title'>{Title}</h5>");
            stringBuilder.Append("<button type='button' class='btn btn-sm btn-icon btn-active-color-primary' data-bs-dismiss='modal'><i class='ki-outline ki-cross fs-1'></i></button>");
            stringBuilder.Append("</div>");

            stringBuilder.Append("<div class='modal-body'>");
            stringBuilder.Append($"<div class='text-gray-800 fs-6'>{Description}</div>");

            stringBuilder.Append("<div class='alert alert-dismissible bg-light-danger border border-danger border-dashed d-flex flex-column flex-sm-row p-5 mb-0 mt-5'>");
            stringBuilder.Append("<i class='ki-outline ki-shield-cross fs-2hx text-danger me-4 mb-5 mb-sm-0 flex-shrink-0'></i>");
            stringBuilder.Append("<div class='d-flex flex-column pe-0 pe-sm-10'>");
            stringBuilder.Append("<h5 class='mb-1 fs-6 text-danger'>Dikkat</h5>");
            stringBuilder.Append("<span class='fs-7 text-gray-600'>Bu işlem kalıcıdır ve bağlı verileri etkileyebilir.</span>");
            stringBuilder.Append("</div></div>");
            stringBuilder.Append("</div>");

            stringBuilder.Append("<div class='modal-footer'>");
            stringBuilder.Append("<button type='button' class='btn btn-light' data-bs-dismiss='modal'>İptal</button>");

            TagBuilder formBuilder = _htmlGenerator.GenerateForm(
                ViewContext,
                Action,
                Controller,
                routeValues: null,
                method: "post",
                htmlAttributes: new { id = $"{Id}Form" });

            TagBuilder inputBuilder = new TagBuilder("input");
            inputBuilder.Attributes.Add("type", "hidden");
            inputBuilder.Attributes.Add("name", "id");
            inputBuilder.Attributes.Add("id", InputId);

            TagBuilder buttonBuilder = new TagBuilder("button");
            buttonBuilder.Attributes.Add("type", "submit");
            buttonBuilder.Attributes.Add("class", "btn btn-danger");
            buttonBuilder.InnerHtml.Append("Evet, Sil");

            formBuilder.InnerHtml.AppendHtml(inputBuilder);
            formBuilder.InnerHtml.AppendHtml(buttonBuilder);

            using (System.IO.StringWriter writer = new System.IO.StringWriter())
            {
                formBuilder.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                stringBuilder.Append(writer.ToString());
            }

            stringBuilder.Append("</div>");
            stringBuilder.Append("</div></div>");

            output.Content.SetHtmlContent(stringBuilder.ToString());
        }
    }
}
