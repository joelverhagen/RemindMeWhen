﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients;
using Knapcode.RemindMeWhen.Core.Compression;
using Knapcode.RemindMeWhen.Core.Extensions;
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

        public async Task<IEnumerable<DocumentMetadata>> ListDocumentMetadataAsync(DocumentId documentId, DateTimeOffset dateTime)
        {
            return await _table.ListAsync(GetDocumentMetadataPartitionKey(documentId), null, dateTime.GetDescendingOrderString());
        }

        public async Task<Document> GetDocumentAsync(DocumentId documentId, string documentMetadataId)
        {
            DocumentMetadata documentMetadata = await GetDocumentMetadataAsync(documentId, documentMetadataId);

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

        public async Task<DocumentMetadata> SaveDocumentAsync(Document document)
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

            // save the metadata, in reverse chronological order
            string documentMetadataId = string.Format(
                CultureInfo.InvariantCulture,
                "{0}-{1}",
                DateTimeOffset.UtcNow.GetDescendingOrderString(),
                Guid.NewGuid());

            var documentMetadata = new DocumentMetadata
            {
                Id = documentMetadataId,
                DocumentId = document.Id,
                Hash = documentHash,
                Duplicate = duplicate,
                Created = DateTimeOffset.UtcNow
            };

            await _table.SetAsync(GetDocumentMetadataPartitionKey(document.Id), documentMetadataId, documentMetadata);

            return documentMetadata;
        }

        private async Task<DocumentMetadata> GetDocumentMetadataAsync(DocumentId documentId, string documentMetadataId)
        {
            // get the metadata
            DocumentMetadata metadata = await _table.GetAsync(GetDocumentMetadataPartitionKey(documentId), documentMetadataId);
            if (metadata == null)
            {
                _eventSource.OnMissingDocumentMetadataFromDocumentStore(documentId, documentMetadataId);
                return null;
            }

            return metadata;
        }

        private string GetDocumentMetadataPartitionKey(DocumentId documentId)
        {
            return _hashAlgorithm.GetHash(Encoding.UTF8.GetBytes(documentId.TypeId));
        }
    }
}