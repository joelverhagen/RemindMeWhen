using System.IO;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Extensions;
using Knapcode.RemindMeWhen.Core.Logging;
using Knapcode.RemindMeWhen.Core.Support;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public class AzureBlobStore : IBlobStore
    {
        private readonly CloudBlobContainer _blobContainer;
        private readonly IEventSource _eventSource;

        public AzureBlobStore(IEventSource eventSource, CloudBlobContainer blobContainer)
        {
            _eventSource = eventSource;
            _blobContainer = blobContainer;
        }

        public async Task<byte[]> GetAsync(string key)
        {
            CloudBlockBlob cloudBlockBlob = _blobContainer.GetBlockBlobReference(key);
            bool exists = await cloudBlockBlob.ExistsAsync();
            if (!exists)
            {
                _eventSource.OnMissingBlobFromAzure(key);
                return null;
            }

            var outputStream = new MemoryStream();
            using (EventTimer.OnCompletion(d => _eventSource.OnBlobDownloadFromAzure(key, outputStream.Length, d)))
            {
                await cloudBlockBlob.DownloadToStreamAsync(outputStream);
            }
            return outputStream.ToArray();
        }

        public async Task SetAsync(string key, byte[] value)
        {
            CloudBlockBlob cloudBlockBlob = _blobContainer.GetBlockBlobReference(key);
            using (EventTimer.OnCompletion(d => _eventSource.OnBlobUploadedToAzure(key, value.LongLength, d)))
            {
                await cloudBlockBlob.UploadFromByteArrayAsync(value, 0, value.Length);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            CloudBlockBlob cloudBlockBlob = _blobContainer.GetBlockBlobReference(key);
            var exists = new Reference<bool>();
            using (EventTimer.OnCompletion(d => _eventSource.OnBlobExistenceCheckedInAzure(key, exists.Value, d)))
            {
                exists.Value = await cloudBlockBlob.ExistsAsync();
            }
            return exists.Value;
        }
    }
}