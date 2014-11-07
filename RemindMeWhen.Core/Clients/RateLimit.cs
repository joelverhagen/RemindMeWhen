using System;

namespace Knapcode.RemindMeWhen.Core.Clients
{
    public class RateLimit
    {
        public int Count { get; set; }
        public TimeSpan Period { get; set; }
    }
}