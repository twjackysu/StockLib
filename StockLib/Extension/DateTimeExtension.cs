using System;

namespace StockLib.DateTimeExtension
{
    public static class DateTimeExtension
    {
        public static ulong ToJavascriptGetTime(this DateTime dateTime)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0);
            var gap = dateTime - start;
            return (ulong)gap.TotalMilliseconds;
        }
    }
}
