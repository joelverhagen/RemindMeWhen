using System.Configuration;

namespace Knapcode.RemindMeWhen.Core.Settings
{
    public class RemindMeWhenConfiguration : ConfigurationSection
    {
        public const string SectionName = "remindMeWhen";

        [ConfigurationProperty("rottenTomatoes")]
        public RottenTomatoesConfiguration RottenTomatoes
        {
            get { return (RottenTomatoesConfiguration) this["rottenTomatoes"]; }
        }

        [ConfigurationProperty("azureStorage")]
        public AzureStorageConfiguration AzureStorage
        {
            get { return (AzureStorageConfiguration) this["azureStorage"]; }
        }

        public static RemindMeWhenConfiguration GetCurrentConfiguration()
        {
            return ConfigurationManager.GetSection(SectionName) as RemindMeWhenConfiguration;
        }

        public static RemindMeWhenSettings GetCurrentSettings()
        {
            RemindMeWhenConfiguration configuration = GetCurrentConfiguration();
            if (configuration == null)
            {
                return null;
            }

            return configuration.GetSettings();
        }

        public RemindMeWhenSettings GetSettings()
        {
            return new RemindMeWhenSettings
            {
                AzureStorage = new AzureStorageSettings
                {
                    ProcessDocumentQueueName = AzureStorage.ProcessDocumentQueueName,
                    ConnectionString = AzureStorage.ConnectionString,
                    DocumentBlobContainerName = AzureStorage.DocumentBlobContainerName,
                    DocumentMetadataTableName = AzureStorage.DocumentMetadataTableName
                },
                RottenTomatoes = new RottenTomatoesSettings
                {
                    ApiKey = RottenTomatoes.ApiKey
                }
            };
        }
    }
}