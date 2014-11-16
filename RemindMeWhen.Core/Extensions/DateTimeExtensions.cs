using System;

namespace Knapcode.RemindMeWhen.Core.Extensions
{
    public static class DateTimeExtensions
    {
        private const string FixedWidthLongFormat = "D19";

        public static string GetDescendingOrderString(this DateTime dateTime)
        {
            return (DateTime.MaxValue.Ticks - dateTime.Ticks).ToString(FixedWidthLongFormat);
        }

        public static string GetAscendingOrderString(this DateTime dateTime)
        {
            return dateTime.Ticks.ToString(FixedWidthLongFormat);
        }
    }
}