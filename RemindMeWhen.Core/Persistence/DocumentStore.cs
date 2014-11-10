using System;
using System.Text;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients;
using Knapcode.RemindMeWhen.Core.Compression;
using Knapcode.RemindMeWhen.Core.Hashing;
using Knapcode.RemindMeWhen.Core.Identities;
using Knapcode.RemindMeWhen.Core.Logging;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public class DocumentStore : IDocumentStore
    {
        private readonly IBlobContainer _blobContainer;
        private readonly ICompressor _compressor;
        private readonly IEventSource _eventSource;
        private readonly IHashAlgorithm _hashAlgorithm;
        private readonly ITable<DocumentMetadata> _table;

        public DocumentStore(IEventSource eventSource, IHashAlgorithm hashAlgorithm, ITable<DocumentMetadata> table, IBlobContainer blobContainer, ICompressor compressor)
        {
            _eventSource = eventSource;
            _hashAlgorithm = hashAlgorithm;
            _table = table;
            _blobContainer = blobContainer;
            _compressor = compressor;
        }

        public async Task<DocumentMetadata> GetDocumentMetadataAsync(DocumentIdentity identity)
        {
            // get the metadata
            string metadataKey = GetMetadataKey(identity);
            DocumentMetadata metadata = await _table.GetAsync(metadataKey);
            if (metadata == null)
            {
                _eventSource.OnMissingDocumentMetadataFromDocumentStore(identity, metadataKey);
                return null;
            }

            return metadata;
        }

        public async Task<Document> GetDocumentAsync(DocumentIdentity identity)
        {
            DocumentMetadata metadata = await GetDocumentMetadataAsync(identity);

            // get the content
            string documentKey = metadata.Hash;
            byte[] compressedContent = await _blobContainer.GetAsync(documentKey);
            if (compressedContent == null)
            {
                _eventSource.OnMissingDocumentFromDocumentStore(identity, documentKey);
                return null;
            }

            // decompress the document
            byte[] decompressedContent = _compressor.Decompress(compressedContent);

            return new Document
            {
                Identity = identity,
                Content = decompressedContent
            };
        }

        public async Task<bool> PersistUniqueDocumentAsync(Document document)
        {
            // save the metadata
            var metadata = new DocumentMetadata
            {
                Type = document.Identity.Type,
                TypeIdentity = document.Identity.TypeIdentity,
                Hash = _hashAlgorithm.GetHash(document.Content),
                LastPersisted = DateTime.UtcNow
            };
            string metadataKey = GetMetadataKey(document.Identity);
            await _table.SetAsync(metadataKey, metadata);

            // save the content if it doesn't already exist
            string documentKey = metadata.Hash;
            if (await _blobContainer.ExistsAsync(documentKey))
            {
                _eventSource.OnDuplicateFoundInDocumentStore(document.Identity, documentKey);
                return true;
            }

            // compress the document
            byte[] compressedContent = _compressor.Compress(document.Content);

            await _blobContainer.SetAsync(documentKey, compressedContent);
            return false;
        }

        private string GetMetadataKey(DocumentIdentity identity)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(identity.ToString());
            return _hashAlgorithm.GetHash(buffer);
        }
    }
}