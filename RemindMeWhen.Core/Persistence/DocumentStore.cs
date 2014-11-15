using System;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients;
using Knapcode.RemindMeWhen.Core.Compression;
using Knapcode.RemindMeWhen.Core.Hashing;
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

        private async Task<DocumentMetadata> GetDocumentMetadataAsync(Guid documentMetadataId)
        {
            // get the metadata
            DocumentMetadata metadata = await _table.GetAsync(documentMetadataId.ToString());
            if (metadata == null)
            {
                _eventSource.OnMissingDocumentMetadataFromDocumentStore(documentMetadataId);
                return null;
            }

            return metadata;
        }

        public async Task<Document> GetDocumentAsync(Guid documentMetadataId)
        {
            DocumentMetadata documentMetadata = await GetDocumentMetadataAsync(documentMetadataId);

            // get the content
            byte[] compressedContent = await _blobContainer.GetAsync(documentMetadata.Hash);
            if (compressedContent == null)
            {
                _eventSource.OnMissingDocumentFromDocumentStore(documentMetadata);
                return null;
            }

            // decompress the document
            byte[] decompressedContent = _compressor.Decompress(compressedContent);

            return new Document
            {
                Id = documentMetadata.DocumentId,
                Content = decompressedContent
            };
        }

        public async Task<DocumentMetadata> PersistUniqueDocumentAsync(Document document)
        {
            // save the content if it doesn't already exist
            string documentHash = _hashAlgorithm.GetHash(document.Content);
            bool duplicate = await _blobContainer.ExistsAsync(documentHash);
            if (duplicate)
            {
                _eventSource.OnDuplicateFoundInDocumentStore(document.Id, documentHash);
            }
            else
            {
                // compress the document
                byte[] compressedContent = _compressor.Compress(document.Content);
                await _blobContainer.SetAsync(documentHash, compressedContent);
            }

            // save the metadata
            Guid documentMetadataId = Guid.NewGuid();
            var documentMetadata = new DocumentMetadata
            {
                Id = documentMetadataId,
                DocumentId = document.Id,
                Hash = documentHash,
                Duplicate = duplicate,
                Created = DateTime.UtcNow
            };

            await _table.SetAsync(documentMetadataId.ToString(), documentMetadata);

            return documentMetadata;
        }
    }
}