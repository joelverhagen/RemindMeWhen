using Knapcode.RemindMeWhen.Core.Persistence;
using Knapcode.RemindMeWhen.Core.Settings;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Ninject;
using Ninject.Modules;

namespace Knapcode.RemindMeWhen.Core.Modules
{
    public class AzureUniqueBlobStoreModule : NinjectModule
    {
        public override void Load()
        {
            var settings = Kernel.Get<RemindMeWhenSettings>();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(settings.AzureStorageSettings.ConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(settings.AzureStorageSettings.ExternalDocumentHashTableName);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobContainer = blobClient.GetContainerReference(settings.AzureStorageSettings.ExternalDocumentBlobContainerName);

            Bind<CloudTable>().ToConstant(table).WhenInjectedInto<AzureUniqueBlobStore>();
            Bind<IHashAlgorithm>().To<Sha256HashAlgorithm>().WhenInjectedInto<AzureUniqueBlobStore>();
            Bind<CloudBlobContainer>().ToConstant(blobContainer).WhenInjectedInto<AzureUniqueBlobStore>();

            Bind<IBlobStore>().To<AzureUniqueBlobStore>();
        }
    }
}