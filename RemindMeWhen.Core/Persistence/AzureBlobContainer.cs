using System;
using System.IO;
using System.Threading.Tasks;
using Knapcode.KitchenSink.Support;
using Knapcode.RemindMeWhen.Core.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public class AzureBlobContainer : IBlobContainer
    {
        private readonly CloudBlobContainer _blobContainer;
        private readonly IEventSource _eventSource;

        public AzureBlobContainer(IEventSource eventSource, CloudBlobContainer blobContainer)
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
            TimeSpan duration = await EventTimer.TimeAsync(async () =>
            {
                await cloudBlockBlob.DownloadToStreamAsync(outputStream);
            });
            _eventSource.OnBlobDownloadFromAzure(key, outputStream.Length, duration);

            return outputStream.ToArray();
        }

        public async Task SetAsync(string key, byte[] value)
        {
            CloudBlockBlob cloudBlockBlob = _blobContainer.GetBlockBlobReference(key);
            TimeSpan duration = await EventTimer.TimeAsync(async () =>
            {
                await cloudBlockBlob.UploadFromByteArrayAsync(value, 0, value.Length);
            });
            _eventSource.OnBlobUploadedToAzure(key, value.LongLength, duration);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            CloudBlockBlob cloudBlockBlob = _blobContainer.GetBlockBlobReference(key);
            bool exists = false;
            TimeSpan duration = await EventTimer.TimeAsync(async () =>
            {
                exists = await cloudBlockBlob.ExistsAsync();
            });
            _eventSource.OnBlobExistenceCheckedInAzure(key, exists, duration);
            return exists;
        }
    }
}