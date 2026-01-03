using System.ComponentModel;
using System.Globalization;

namespace TKH.Core.Common
{
    public static class CommonHelper
    {
        public static T To<T>(object value)
        {
            return (T)To(value, typeof(T));
        }

        public static object To(object value, Type destinationType)
        {
            if (value is null)
                return destinationType.IsValueType ? Activator.CreateInstance(destinationType)! : null!;

            Type sourceType = value.GetType();

            if (destinationType.IsAssignableFrom(sourceType))
                return value;

            TypeConverter destinationConverter = TypeDescriptor.GetConverter(destinationType);
            TypeConverter sourceConverter = TypeDescriptor.GetConverter(sourceType);

            if (destinationConverter is not null && destinationConverter.CanConvertFrom(value.GetType()))
                return destinationConverter.ConvertFrom(null, CultureInfo.InvariantCulture, value)!;

            if (sourceConverter is not null && sourceConverter.CanConvertTo(destinationType))
                return sourceConverter.ConvertTo(null, CultureInfo.InvariantCulture, value, destinationType)!;

            if (destinationType.IsEnum && value is int)
                return Enum.ToObject(destinationType, (int)value);

            if (!destinationType.IsInstanceOfType(value))
                return Convert.ChangeType(value, destinationType, CultureInfo.InvariantCulture);

            return value;
        }
    }
}
