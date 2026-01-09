using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TKH.Web.Infrastructure.TagHelpers
{
    [HtmlTargetElement("app-filter-select", Attributes = "asp-for")]
    public class FilterSelectTagHelper : TagHelper
    {
        private readonly IHtmlGenerator _generator;

        public FilterSelectTagHelper(IHtmlGenerator generator)
        {
            _generator = generator;
        }

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("asp-items")]
        public IEnumerable<SelectListItem>? Items { get; set; }

        [HtmlAttributeName("true-text")]
        public string? TrueText { get; set; }

        [HtmlAttributeName("false-text")]
        public string? FalseText { get; set; }

        [HtmlAttributeName("all-text")]
        public string AllText { get; set; } = "Tümü";

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext tagHelperContext, TagHelperOutput tagHelperOutput)
        {

            Type modelType = Nullable.GetUnderlyingType(For.ModelExplorer.ModelType) ?? For.ModelExplorer.ModelType;

            IEnumerable<SelectListItem> selectListItems = Items switch
            {
                not null => CreateCustomSelectList(Items, AllText),

                _ when modelType.IsEnum => CreateEnumSelectList(modelType, AllText),

                _ when modelType == typeof(bool) => CreateBoolSelectList(TrueText!, FalseText!, AllText),

                _ => throw new InvalidOperationException("Unsupported filter type. Please provide asp-items for custom lists.")
            };


            string? propertyName = For.Metadata.Name;

            FromQueryAttribute? fromQueryAttribute = For.Metadata.ContainerMetadata?.ModelType.GetProperty(For.Metadata.PropertyName!)?.GetCustomAttribute<FromQueryAttribute>();

            if (fromQueryAttribute is not null && !string.IsNullOrEmpty(fromQueryAttribute.Name))
                propertyName = fromQueryAttribute.Name;

            var htmlAttributes = tagHelperOutput.Attributes.ToDictionary(attribute => attribute.Name, attribute => attribute.Value);

            TagBuilder tagBuilder = _generator.GenerateSelect(ViewContext!, For!.ModelExplorer, optionLabel: null, expression: propertyName, selectList: selectListItems, allowMultiple: false, htmlAttributes: htmlAttributes);

            tagHelperOutput.TagName = null;
            tagHelperOutput.Content.SetHtmlContent(tagBuilder);
        }

        private static IEnumerable<SelectListItem> CreateCustomSelectList(IEnumerable<SelectListItem> items, string? allText)
        {
            List<SelectListItem> selectListItems = new();

            if (!string.IsNullOrEmpty(allText))
                selectListItems.Add(new SelectListItem { Value = "", Text = allText });

            selectListItems.AddRange(items);
            return selectListItems;
        }

        public static IEnumerable<SelectListItem> CreateEnumSelectList(Type enumType, string allText)
        {
            yield return new SelectListItem { Value = "", Text = allText };

            foreach (var value in Enum.GetValues(enumType))
            {
                MemberInfo memberInfo = enumType.GetMember(value!.ToString()!).First();
                string? displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>()?.Name ?? value.ToString();

                yield return new SelectListItem
                {
                    Value = Convert.ToInt32(value).ToString(),
                    Text = displayAttribute
                };
            }
        }

        public static IEnumerable<SelectListItem> CreateBoolSelectList(string trueText, string falseText, string allText)
        {
            return new[]
            {
                new SelectListItem { Value = "", Text = allText },
                new SelectListItem { Value = "true", Text = trueText ?? "Evet" },
                new SelectListItem { Value = "false", Text = falseText ?? "Hayır" }
            };
        }
    }
}
