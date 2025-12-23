using System.Text.RegularExpressions;

namespace TKH.Business.Extensions
{
    public static class StorageExtensions
    {
        public static string ToUrlSlug(this string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return "";

            string str = phrase.ToLowerInvariant();

            str = str.Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                     .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c");

            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            str = Regex.Replace(str, @"\s+", "-").Trim();

            str = Regex.Replace(str, @"-+", "-");

            return str;
        }
    }
}
