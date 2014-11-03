using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes;

namespace Knapcode.RemindMeWhen.Core.Settings
{
    public class RemindMeWhenSettings
    {
        public RottenTomatoesClientSettings RottenTomatoesClientSettings { get; set; }
        public AzureStorageSettings AzureStorageSettings { get; set; }
    }
}