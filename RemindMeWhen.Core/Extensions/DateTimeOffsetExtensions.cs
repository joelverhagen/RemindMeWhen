using System;

namespace Knapcode.RemindMeWhen.Core.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        private const string FixedWidthLongFormat = "D19";

        public static string GetDescendingOrderString(this DateTimeOffset dateTime)
        {
            return (DateTimeOffset.MaxValue.Ticks - dateTime.Ticks).ToString(FixedWidthLongFormat);
        }

        public static string GetAscendingOrderString(this DateTimeOffset dateTime)
        {
            return dateTime.Ticks.ToString(FixedWidthLongFormat);
        }
    }
}