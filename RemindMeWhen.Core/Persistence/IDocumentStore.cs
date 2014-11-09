using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients;
using Knapcode.RemindMeWhen.Core.Identities;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public interface IDocumentStore
    {
        /// <summary>
        /// Geth the document metadata with the provided identity.
        /// </summary>
        /// <param name="identity">The document identity.</param>
        /// <returns>The document metadata. Returns null if it does not exist.</returns>
        Task<DocumentMetadata> GetDocumentMetadataAsync(DocumentIdentity identity);

        /// <summary>
        /// Get the document with the provided identity.
        /// </summary>
        /// <param name="identity">The document identity.</param>
        /// <returns>The document. Returns null if it does not exists.</returns>
        Task<Document> GetDocumentAsync(DocumentIdentity identity);

        /// <summary>
        /// Persist the provided document to the document store.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>A value indicating whether the provided document is a duplicate.</returns>
        Task<bool> PersistUniqueDocumentAsync(Document document);
    }
}