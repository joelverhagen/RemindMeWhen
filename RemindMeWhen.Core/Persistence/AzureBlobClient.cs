using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public class AzureBlobClient : IBlobClient
    {
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly IEventSource _eventSource;

        public AzureBlobClient(IEventSource eventSource, CloudBlobClient cloudBlobClient)
        {
            _eventSource = eventSource;
            _cloudBlobClient = cloudBlobClient;
        }

        public IBlobContainer GetBlobContainer(string blobContainerName)
        {
            return new AzureBlobContainer(_eventSource, _cloudBlobClient.GetContainerReference(blobContainerName));
        }
    }
}