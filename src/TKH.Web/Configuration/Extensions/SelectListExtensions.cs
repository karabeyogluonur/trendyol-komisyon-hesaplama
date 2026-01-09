using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TKH.Web.Configuration.Extensions
{
    public static class SelectListExtensions
    {
        public static List<SelectListItem> ToSelectList<T>(this IEnumerable<T> items, Func<T, string> valueSelector, Func<T, string> textSelector, string? selectedValue = null)
        {
            return items.Select(item => new SelectListItem
            {
                Value = valueSelector(item),
                Text = textSelector(item),
                Selected = valueSelector(item) == selectedValue
            }).ToList();
        }

    }

}
