using System.Globalization;

namespace TKH.Web.Configuration.Extensions
{
    public static class NumberExtensions
    {
        public static string ToJsFormat(this decimal value)
        {
            return value.ToString("F2", CultureInfo.InvariantCulture);
        }
        public static string ToJsFormat(this decimal? value, string defaultValue = "0.00")
        {
            return value.HasValue
                ? value.Value.ToString("F2", CultureInfo.InvariantCulture)
                : defaultValue;
        }
        public static string ToNullableJsFormat(this decimal? value)
        {
            return value?.ToString("F2", CultureInfo.InvariantCulture);
        }

        public static string ToPercentageRate(this decimal value)
        {
            return (value / 100m).ToString("F2", CultureInfo.InvariantCulture);
        }
    }
}
