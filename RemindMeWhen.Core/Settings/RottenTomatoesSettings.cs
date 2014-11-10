namespace Knapcode.RemindMeWhen.Core.Settings
{
    public class RottenTomatoesSettings
    {
        /*
         * TODO: add this as a setting
         * RateLimits = new[]
         * {
         *      new RateLimit {Count = 5, Period = TimeSpan.FromSeconds(1)},
         *      new RateLimit {Count = 10000, Period = TimeSpan.FromDays(1)}
         * };
         */

        public string ApiKey { get; set; }
    }
}