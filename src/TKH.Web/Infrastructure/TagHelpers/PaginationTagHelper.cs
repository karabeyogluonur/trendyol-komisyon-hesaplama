using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace TKH.Web.Infrastructure.TagHelpers
{
    [HtmlTargetElement("app-pagination")]
    public class PaginationTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PaginationTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public dynamic Model { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string FormId { get; set; } = "filterForm";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Model is null || Model.TotalCount == 0)
            {
                output.SuppressOutput();
                return;
            }

            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<div class='row pt-10'>");

            stringBuilder.Append("<div class='col-sm-12 col-md-6 d-flex align-items-center justify-content-center justify-content-md-start gap-3'>");

            stringBuilder.Append("<div class='dataTables_length'>");

            stringBuilder.Append($"<select name='PageSize' form='{FormId}' onchange='this.form.submit()' class='form-select form-select-sm form-select-solid w-75px'>");

            int[] sizeOptions = new int[] { 10, 20, 50, 100 };
            foreach (int size in sizeOptions)
            {
                string selected = (Model.PageSize == size) ? "selected" : "";
                stringBuilder.Append($"<option value='{size}' {selected}>{size}</option>");
            }

            stringBuilder.Append("</select>");
            stringBuilder.Append("</div>");

            stringBuilder.Append("<div class='dataTables_info text-gray-600 fs-7'>");
            int startRecord = (Model.PageIndex - 1) * Model.PageSize + 1;
            int endRecord = Math.Min(Model.PageIndex * Model.PageSize, (int)Model.TotalCount);
            stringBuilder.Append($"Toplam <strong>{Model.TotalCount}</strong> kayıttan <strong>{startRecord} - {endRecord}</strong> arası gösteriliyor.");
            stringBuilder.Append("</div>");

            stringBuilder.Append("</div>");

            stringBuilder.Append("<div class='col-sm-12 col-md-6 d-flex align-items-center justify-content-center justify-content-md-end'>");
            stringBuilder.Append("<div class='dataTables_paginate paging_simple_numbers'><ul class='pagination'>");

            bool hasPreviousPage = Model.HasPreviousPage;
            string previousUrl = hasPreviousPage ? GenerateUrl(urlHelper, Model.PageIndex - 1) : "#";
            string previousClass = hasPreviousPage ? "" : "disabled";
            stringBuilder.Append($"<li class='page-item previous {previousClass}'><a href='{previousUrl}' class='page-link'><i class='previous'></i></a></li>");

            int totalPageCount = Model.TotalPages;
            int currentPageIndex = Model.PageIndex;

            for (int pageNumber = 1; pageNumber <= totalPageCount; pageNumber++)
            {
                if (pageNumber == 1 || pageNumber == totalPageCount || (pageNumber >= currentPageIndex - 2 && pageNumber <= currentPageIndex + 2))
                {
                    string activeClass = (pageNumber == currentPageIndex) ? "active" : "";
                    string generatedUrl = GenerateUrl(urlHelper, pageNumber);
                    stringBuilder.Append($"<li class='page-item {activeClass}'><a href='{generatedUrl}' class='page-link'>{pageNumber}</a></li>");
                }
                else if (pageNumber == currentPageIndex - 3 || pageNumber == currentPageIndex + 3)
                    stringBuilder.Append("<li class='page-item disabled'><span class='page-link'>...</span></li>");
            }

            bool hasNextPage = Model.HasNextPage;
            string nextUrl = hasNextPage ? GenerateUrl(urlHelper, Model.PageIndex + 1) : "#";
            string nextClass = hasNextPage ? "" : "disabled";
            stringBuilder.Append($"<li class='page-item next {nextClass}'><a href='{nextUrl}' class='page-link'><i class='next'></i></a></li>");

            stringBuilder.Append("</ul></div></div></div>");

            output.TagName = null;
            output.Content.SetHtmlContent(stringBuilder.ToString());
        }

        private string GenerateUrl(IUrlHelper urlHelper, int targetPageNumber)
        {
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();

            foreach (string key in ViewContext.HttpContext.Request.Query.Keys)
                queryParameters[key] = ViewContext.HttpContext.Request.Query[key];

            queryParameters["PageIndex"] = targetPageNumber.ToString();
            queryParameters["PageSize"] = Model.PageSize.ToString();

            return urlHelper.Action(Action, Controller, queryParameters);
        }
    }
}
