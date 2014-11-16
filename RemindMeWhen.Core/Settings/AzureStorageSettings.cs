namespace Knapcode.RemindMeWhen.Core.Settings
{
    public class AzureStorageSettings
    {
        public string ConnectionString { get; set; }
        public string DocumentMetadataTableName { get; set; }
        public string DocumentBlobContainerName { get; set; }
        public string ProcessDocumentQueueName { get; set; }
        public string SubscriptionTableName { get; set; }
    }
}