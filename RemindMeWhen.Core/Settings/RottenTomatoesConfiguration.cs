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
    }
}