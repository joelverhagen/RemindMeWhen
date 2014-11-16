using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public interface IDocumentStore
    {
        /// <summary>
        /// Get the list of all document metadata for the provided document ID.
        /// </summary>
        /// <param name="documentId">The document ID.</param>
        /// <param name="after">Only document metadata created after this time will be returned.</param>
        /// <returns>The list of document metadata.</returns>
        Task<IEnumerable<DocumentMetadata>> ListDocumentMetadataAsync(DocumentId documentId, DateTime after);

        /// <summary>
        /// Get the document with the provided document metadata ID.
        /// </summary>
        /// <param name="documentId">The document ID.</param>
        /// <param name="documentMetadataId">The document metadata ID.</param>
        /// <returns>The document. Returns null if it does not exists.</returns>
        Task<Document> GetDocumentAsync(DocumentId documentId, string documentMetadataId);

        /// <summary>
        /// Persist the provided document to the document store.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>A value indicating whether the provided document is a duplicate.</returns>
        Task<DocumentMetadata> SaveDocumentAsync(Document document);
    }
}