using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public class AzureUniqueBlobStore : IBlobStore
    {
        private readonly CloudBlobContainer _blobContainer;
        private readonly IHashAlgorithm _hashAlgorithm;
        private readonly CloudTable _table;

        public AzureUniqueBlobStore(CloudTable table, IHashAlgorithm hashAlgorithm, CloudBlobContainer blobContainer)
        {
            _table = table;
            _hashAlgorithm = hashAlgorithm;
            _blobContainer = blobContainer;
        }

        public async Task<byte[]> Get(string key)
        {
            // try to get the hash 
            TableOperation operation = TableOperation.Retrieve<HashTableEntity>(key, key);
            TableResult tableResult = await _table.ExecuteAsync(operation);
            var hashTableEntity = tableResult.Result as HashTableEntity;
            if (hashTableEntity == null)
            {
                return null;
            }

            // get the blob if it exists
            string blobKey = hashTableEntity.Hash.ToString();
            ICloudBlob blob = await _blobContainer.GetBlobReferenceFromServerAsync(blobKey);
            if (!(await blob.ExistsAsync()))
            {
                return null;
            }

            var stream = new MemoryStream();
            await blob.DownloadToStreamAsync(stream);
            return stream.ToArray();
        }

        public async Task Set(string key, byte[] value)
        {
            // get a hash of the blob
            Hash hash = _hashAlgorithm.GetHash(value);

            // associate the key with the hash
            var hashTableEntity = new HashTableEntity
            {
                PartitionKey = key,
                RowKey = key,
                Hash = hash,
                IsCompressed = false
            };
            TableOperation operation = TableOperation.InsertOrReplace(hashTableEntity);
            await _table.ExecuteAsync(operation);

            // persist the blob if it doesn't exist yet
            string blobKey = hash.ToString();
            ICloudBlob blob = await _blobContainer.GetBlobReferenceFromServerAsync(blobKey);
            if (await blob.ExistsAsync())
            {
                return;
            }

            await blob.UploadFromByteArrayAsync(value, 0, value.Length);
        }

        private class HashTableEntity : TableEntity
        {
            public Hash Hash { get; set; }
            public bool IsCompressed { get; set; }
        }
    }
}