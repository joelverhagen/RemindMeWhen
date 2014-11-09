using System;
using System.Collections.Generic;
using Knapcode.RemindMeWhen.Core.Clients;

namespace Knapcode.RemindMeWhen.Core.Settings
{
    public class RottenTomatoesSettings
    {
        public RottenTomatoesSettings()
        {
            RateLimits = new[]
            {
                new RateLimit {Count = 5, Period = TimeSpan.FromSeconds(1)},
                new RateLimit {Count = 10000, Period = TimeSpan.FromDays(1)}
            };
        }

        public string Key { get; set; }
        public IEnumerable<RateLimit> RateLimits { get; set; }
    }
}