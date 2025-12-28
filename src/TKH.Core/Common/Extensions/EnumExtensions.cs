using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TKH.Core.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            if (enumValue is null) return string.Empty;

            DisplayAttribute? displayAttribute = enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<DisplayAttribute>();

            if (displayAttribute is not null && !string.IsNullOrEmpty(displayAttribute.GetName()))
                return displayAttribute.GetName();

            DescriptionAttribute? descriptionAttribute = enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<DescriptionAttribute>();

            if (descriptionAttribute != null)
                return descriptionAttribute.Description;

            return enumValue.ToString();
        }
    }
}
