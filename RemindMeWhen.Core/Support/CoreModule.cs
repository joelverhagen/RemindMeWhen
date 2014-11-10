using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes;
using Knapcode.RemindMeWhen.Core.Extensions;
using Knapcode.RemindMeWhen.Core.Persistence;
using Knapcode.RemindMeWhen.Core.Repositories;
using Knapcode.RemindMeWhen.Core.Settings;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace Knapcode.RemindMeWhen.Core.Support
{
    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            // load settings
            RemindMeWhenSettings settings = RemindMeWhenConfiguration.GetCurrentSettings();
            Bind<RemindMeWhenSettings>().ToConstant(settings);
            Bind<AzureStorageSettings>().ToConstant(settings.AzureStorage);
            Bind<RottenTomatoesSettings>().ToConstant(settings.RottenTomatoes);

            // bind everything in this assembly
            Kernel.Bind(x => x.FromThisAssembly().SelectAllClasses().BindAllInterfaces());

            // AzureStorage
            // TODO: create the Azure resources somewhere else...
            AzureStorageSettings azureStorageSettings = settings.AzureStorage;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureStorageSettings.ConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            CloudTable documentMetadataTable = tableClient.GetTableReference(azureStorageSettings.DocumentMetadataTableName);
            documentMetadataTable.CreateIfNotExists();
            Bind<CloudTable>().ToConstant(documentMetadataTable).WhenInjectedIntoDescendentOf(typeof (DocumentStore));

            CloudBlobContainer documentBlobContainer = blobClient.GetContainerReference(azureStorageSettings.DocumentBlobContainerName);
            documentBlobContainer.CreateIfNotExists();
            Bind<CloudBlobContainer>().ToConstant(documentBlobContainer).WhenInjectedIntoDescendentOf(typeof (DocumentStore));

            CloudQueue processDocumentQueue = queueClient.GetQueueReference(azureStorageSettings.ProcessDocumentQueueName);
            processDocumentQueue.CreateIfNotExists();
            Bind<CloudQueue>().ToConstant(processDocumentQueue).WhenInjectedIntoDescendentOf(typeof (RottenTomatoesRepository));
        }
    }
}