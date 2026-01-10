namespace TKH.Core.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime EnsureUtc(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime;

            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        public static DateTime? EnsureUtc(this DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return null;

            return dateTime.Value.EnsureUtc();
        }
    }
}
