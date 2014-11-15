using System.Linq;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes;
using Knapcode.RemindMeWhen.Core.Models;
using Knapcode.RemindMeWhen.Core.Persistence;
using Knapcode.RemindMeWhen.Core.Queue;

namespace Knapcode.RemindMeWhen.Core.Repositories
{
    public class RottenTomatoesRepository<T> : IRottenTomatoesRepository<T> where T : MovieReleasedEvent
    {
        private readonly IDocumentStore _documentStore;
        private readonly IEventExtractor<byte[], T> _eventExtractor;
        private readonly IRottenTomatoesDocumentClient _externalDocumentClient;
        private readonly IQueue<ProcessDocument> _queue;

        public RottenTomatoesRepository(IRottenTomatoesDocumentClient externalDocumentClient, IDocumentStore documentStore, IQueue<ProcessDocument> queue, IEventExtractor<byte[], T> eventExtractor)
        {
            _externalDocumentClient = externalDocumentClient;
            _documentStore = documentStore;
            _queue = queue;
            _eventExtractor = eventExtractor;
        }

        public async Task<Page<T>> SearchMovieReleaseEventsAsync(string query, PageOffset pageOffset)
        {
            // query Rotten Tomatoes API
            Document document = await _externalDocumentClient.SearchMoviesAsync(query, pageOffset);

            // persist the document
            DocumentMetadata documentMetadata = await _documentStore.PersistUniqueDocumentAsync(document);

            // enqueue the process queue message if the document is new
            if (!documentMetadata.Duplicate)
            {
                await _queue.AddMessageAsync(new ProcessDocument {DocumentMetadata = documentMetadata});
            }

            // extract the events
            T[] entries = _eventExtractor.Extract(document.Content).ToArray();

            return new Page<T>
            {
                Offset = pageOffset,
                Entries = entries,
                HasNextPage = entries.Length >= pageOffset.Size
            };
        }
    }
}