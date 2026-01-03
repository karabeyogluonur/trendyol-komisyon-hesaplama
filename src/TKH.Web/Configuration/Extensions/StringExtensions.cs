namespace TKH.Web.Configuration.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(value)) return value;

            return value.Length <= maxLength
                ? value
                : $"{value.Substring(0, maxLength - suffix.Length)}{suffix}";
        }
    }
}
