namespace Knapcode.RemindMeWhen.Core.Settings
{
    public class AzureStorageSettings
    {
        public string ConnectionString { get; set; }
        public string ExternalDocumentHashTableName { get; set; }
        public string ExternalDocumentBlobContainerName { get; set; }
        public AzureQueueSettings QueueSettings { get; set; }
    }
}