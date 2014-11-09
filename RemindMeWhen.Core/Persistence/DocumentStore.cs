using System;
using System.Text;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients;
using Knapcode.RemindMeWhen.Core.Hashing;
using Knapcode.RemindMeWhen.Core.Identities;
using Knapcode.RemindMeWhen.Core.Logging;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public class DocumentStore : IDocumentStore
    {
        private readonly IBlobContainer _blobContainer;
        private readonly IEventSource _eventSource;
        private readonly IHashAlgorithm _hashAlgorithm;
        private readonly ITable<DocumentMetadata> _table;

        public DocumentStore(IEventSource eventSource, IHashAlgorithm hashAlgorithm, ITable<DocumentMetadata> table, IBlobContainer blobContainer)
        {
            _eventSource = eventSource;
            _hashAlgorithm = hashAlgorithm;
            _table = table;
            _blobContainer = blobContainer;
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
            byte[] content = await _blobContainer.GetAsync(documentKey);
            if (content == null)
            {
                _eventSource.OnMissingDocumentFromDocumentStore(identity, documentKey);
                return null;
            }

            return new Document
            {
                Identity = identity,
                Content = content
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

            await _blobContainer.SetAsync(documentKey, document.Content);
            return false;
        }

        private string GetMetadataKey(DocumentIdentity identity)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(identity.ToString());
            return _hashAlgorithm.GetHash(buffer);
        }
    }
}