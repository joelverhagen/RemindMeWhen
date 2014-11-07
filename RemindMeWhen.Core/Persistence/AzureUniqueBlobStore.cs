using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Support;
using Knapcode.RemindMeWhen.Core.Support.Extensions;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public class AzureUniqueBlobStore : IBlobStore
    {
        private readonly CloudBlobContainer _cloudBlobContainer;
        private readonly IHashAlgorithm _hashAlgorithm;
        private readonly CloudTable _cloudTable;

        public AzureUniqueBlobStore(CloudTable cloudTable, IHashAlgorithm hashAlgorithm, CloudBlobContainer cloudBlobContainer)
        {
            _cloudTable = cloudTable;
            _hashAlgorithm = hashAlgorithm;
            _cloudBlobContainer = cloudBlobContainer;
        }

        public async Task<byte[]> GetAsync(string key)
        {
            // try to get the hash 
            TableOperation operation = TableOperation.Retrieve<BlobEntity>(key, key);
            TableResult tableResult = await _cloudTable.ExecuteAsync(operation);
            var hashTableEntity = tableResult.Result as BlobEntity;
            if (hashTableEntity == null)
            {
                return null;
            }

            // get the blob if it exists
            string blobKey = hashTableEntity.BlobHash;
            ICloudBlob blob = await _cloudBlobContainer.GetBlobReferenceFromServerAsync(blobKey);
            if (!(await blob.ExistsAsync()))
            {
                return null;
            }

            // download and decompress the document
            var downloadedStream = new MemoryStream();
            await blob.DownloadToStreamAsync(downloadedStream);
            return downloadedStream.ToArray().Decompress();
        }

        public async Task SetAsync(string key, byte[] value)
        {
            // associate the key with the hash
            var blobEntity = new BlobEntity
            {
                PartitionKey = GetPartitionKey(key),
                RowKey = Guid.NewGuid().ToString(),
                OriginalKey = key,
                BlobHash = _hashAlgorithm.GetHash(value).ToString(),
                Created = DateTime.UtcNow
            };
            TableOperation operation = TableOperation.InsertOrReplace(blobEntity);
            await _cloudTable.ExecuteAsync(operation);

            // persist the blob if it doesn't exist yet
            string blobKey = blobEntity.BlobHash;
            ICloudBlob blob = _cloudBlobContainer.GetBlockBlobReference(blobKey);
            if (await blob.ExistsAsync())
            {
                return;
            }

            // compress and upload the document
            byte[] compressedValue = value.Compress();
            await blob.UploadFromByteArrayAsync(compressedValue, 0, compressedValue.Length);
        }

        private string GetPartitionKey(string identifier)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(identifier);
            return _hashAlgorithm.GetHash(buffer).ToString();
        }

        private class BlobEntity : TableEntity
        {
            public string OriginalKey { get; set; }
            public string BlobHash { get; set; }
            public DateTime Created { get; set; }
        }
    }
}