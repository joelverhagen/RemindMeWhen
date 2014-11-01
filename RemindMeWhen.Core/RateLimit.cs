using System;

namespace Knapcode.RemindMeWhen.Core
{
    public class RateLimit
    {
        public int Count { get; set; }
        public TimeSpan Period { get; set; }
    }
}