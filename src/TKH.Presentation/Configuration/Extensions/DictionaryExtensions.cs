using System.Globalization;

namespace TKH.Presentation.Configuration.Extensions
{
    public static class DictionaryExtensions
    {
        public static string GetJsDecimal(this Dictionary<string, object> dictionary, string key, decimal defaultValue = 0)
        {
            if (dictionary != null && dictionary.TryGetValue(key, out object value))
            {
                try
                {
                    decimal decimalValue = Convert.ToDecimal(value);
                    return decimalValue.ToString("F2", CultureInfo.InvariantCulture);
                }
                catch
                {

                }
            }
            return defaultValue.ToString("F2", CultureInfo.InvariantCulture);
        }
    }
}
