using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes;
using Knapcode.RemindMeWhen.Core.Hashing;
using Knapcode.RemindMeWhen.Core.Persistence;
using Knapcode.RemindMeWhen.Core.Settings;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Ninject;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace Knapcode.RemindMeWhen.Core.Modules
{
    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(x => x.FromThisAssembly());

            // load settings
            var settings = Kernel.Get<RemindMeWhenSettings>();

            // AzureUniqueBlobStore settings
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(settings.AzureStorageSettings.ConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(settings.AzureStorageSettings.ExternalDocumentHashTableName);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(settings.AzureStorageSettings.ExternalDocumentBlobContainerName);

            // RottenTomatoesDocumentClient settings
            Bind<RottenTomatoesSettings>().ToConstant(settings.RottenTomatoesSettings).WhenInjectedInto<RottenTomatoesDocumentClient>();
        }
    }
}