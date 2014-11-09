using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes;
using Knapcode.RemindMeWhen.Core.Extensions;
using Knapcode.RemindMeWhen.Core.Persistence;
using Knapcode.RemindMeWhen.Core.Repositories;
using Knapcode.RemindMeWhen.Core.Settings;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace Knapcode.RemindMeWhen.Core.Support
{
    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(x => x.FromThisAssembly().SelectAllClasses().BindAllInterfaces());

            // load settings
            var settings = Kernel.Get<RemindMeWhenSettings>();

            // AzureStorageSettings
            AzureStorageSettings azureStorageSettings = settings.AzureStorageSettings;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureStorageSettings.ConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            CloudTable documentMetadataTable = tableClient.GetTableReference(azureStorageSettings.DocumentMetadataTableName);
            Bind<CloudTable>().ToConstant(documentMetadataTable).WhenInjectedIntoDescendentOf(typeof(DocumentStore));

            CloudBlobContainer documentBlobContainer = blobClient.GetContainerReference(azureStorageSettings.DocumentBlobContainerName);
            Bind<CloudBlobContainer>().ToConstant(documentBlobContainer).WhenInjectedIntoDescendentOf(typeof(DocumentStore));

            CloudQueue processDocumentQueue = queueClient.GetQueueReference(azureStorageSettings.ProcessDocumentQueueName);
            Bind<CloudQueue>().ToConstant(processDocumentQueue).WhenInjectedIntoDescendentOf(typeof(RottenTomatoesRepository));

            // RottenTomatoesSettings
            Bind<RottenTomatoesSettings>().ToConstant(settings.RottenTomatoesSettings).WhenInjectedInto(typeof(RottenTomatoesDocumentClient));
        }
    }
}