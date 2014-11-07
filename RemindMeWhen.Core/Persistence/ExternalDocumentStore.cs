using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public class ExternalDocumentStore : IExternalDocumentStore
    {
        private readonly IBlobStore _blobStore;

        public ExternalDocumentStore(IBlobStore blobStore)
        {
            _blobStore = blobStore;
        }

        public async Task<ExternalDocument> GetAsync(string identity)
        {
            byte[] content = await _blobStore.GetAsync(identity);
            return new ExternalDocument
            {
                Identity = identity,
                Content = content
            };
        }

        public async Task SetAsync(ExternalDocument document)
        {
            await _blobStore.SetAsync(document.Identity, document.Content);
        }
    }
}