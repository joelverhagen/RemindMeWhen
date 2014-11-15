using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients;
using System;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public interface IDocumentStore
    {
        /// <summary>
        /// Get the document with the provided document metadata ID.
        /// </summary>
        /// <param name="documentMetadataId">The document metadata ID.</param>
        /// <returns>The document. Returns null if it does not exists.</returns>
        Task<Document> GetDocumentAsync(Guid documentMetadataId);

        /// <summary>
        /// Persist the provided document to the document store.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>A value indicating whether the provided document is a duplicate.</returns>
        Task<DocumentMetadata> PersistUniqueDocumentAsync(Document document);
    }
}