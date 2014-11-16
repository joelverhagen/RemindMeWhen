using System;
using System.Configuration;

namespace Knapcode.RemindMeWhen.Core.Settings
{
    public class RottenTomatoesConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("apiKey")]
        public string ApiKey
        {
            get { return (string) this["apiKey"]; }
        }

        [ConfigurationProperty("documentCacheDuration")]
        private string DocumentCacheDurationString
        {
            get { return (string) this["documentCacheDuration"]; }
        }

        public TimeSpan DocumentCacheDuration
        {
            get
            {
                TimeSpan output;
                if (!TimeSpan.TryParse(DocumentCacheDurationString, out output))
                {
                    output = new TimeSpan(1, 0, 0, 0);
                }

                return output;
            }
        }
    }
}