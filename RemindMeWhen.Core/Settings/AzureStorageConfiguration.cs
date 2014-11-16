using System.Configuration;

namespace Knapcode.RemindMeWhen.Core.Settings
{
    public class AzureStorageConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("connectionString")]
        public string ConnectionString
        {
            get { return (string) this["connectionString"]; }
        }

        [ConfigurationProperty("documentMetadataTableName")]
        public string DocumentMetadataTableName
        {
            get { return (string) this["documentMetadataTableName"]; }
        }

        [ConfigurationProperty("documentBlobContainerName")]
        public string DocumentBlobContainerName
        {
            get { return (string) this["documentBlobContainerName"]; }
        }

        [ConfigurationProperty("processDocumentQueueName")]
        public string ProcessDocumentQueueName
        {
            get { return (string) this["processDocumentQueueName"]; }
        }

        [ConfigurationProperty("subscriptionTableName")]
        public string SubscriptionTableName
        {
            get { return (string) this["subscriptionTableName"]; }
        }

        [ConfigurationProperty("saveSubscriptionQueueName")]
        public string SaveSubscriptionQueueName
        {
            get { return (string) this["saveSubscriptionQueueName"]; }
        }
    }
}