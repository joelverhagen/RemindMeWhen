using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes;
using Knapcode.RemindMeWhen.Core.Models;
using Knapcode.RemindMeWhen.Core.Persistence;
using Knapcode.RemindMeWhen.Core.Queue;
using Knapcode.RemindMeWhen.Core.Settings;

namespace Knapcode.RemindMeWhen.Core.Repositories
{
    public class RottenTomatoesRepository<T> : IRottenTomatoesRepository<T> where T : MovieReleasedEvent
    {
        private readonly IDocumentStore _documentStore;
        private readonly IEventExtractor<byte[], T> _eventExtractor;
        private readonly IRottenTomatoesDocumentClient _externalDocumentClient;
        private readonly IQueue<ProcessDocumentMessage> _queue;
        private readonly TimeSpan _documentCacheDuration;

        public RottenTomatoesRepository(IRottenTomatoesDocumentClient externalDocumentClient, IDocumentStore documentStore, IQueue<ProcessDocumentMessage> queue, IEventExtractor<byte[], T> eventExtractor, RottenTomatoesSettings settings)
        {
            _externalDocumentClient = externalDocumentClient;
            _documentStore = documentStore;
            _queue = queue;
            _eventExtractor = eventExtractor;
            _documentCacheDuration = settings.DocumentCacheDuration;
        }

        public async Task<Page<T>> SearchMovieReleaseEventsAsync(string query, PageOffset pageOffset)
        {
            // check for a recent search
            DocumentId documentId = _externalDocumentClient.SearchMovies(query, pageOffset);
            IEnumerable<DocumentMetadata> documentMetadataList = await _documentStore.ListDocumentMetadataAsync(documentId, DateTimeOffset.UtcNow.Subtract(_documentCacheDuration));
            DocumentMetadata[] documentMetadataArray = documentMetadataList.ToArray();

            // get the document from the cache or from the external source
            Document document;
            if (documentMetadataArray.Any())
            {
                // get the latest document from the cache
                DocumentMetadata documentMetadata = documentMetadataArray.OrderByDescending(m => m.Created).First();
                document = await _documentStore.GetDocumentAsync(documentId, documentMetadata.Id);
            }
            else
            {
                // query Rotten Tomatoes API
                document = await _externalDocumentClient.GetDocumentAsync(documentId);

                // persist the document
                DocumentMetadata documentMetadata = await _documentStore.SaveDocumentAsync(document);

                // enqueue the process queue message if the document is new
                if (!documentMetadata.Duplicate)
                {
                    await _queue.AddMessageAsync(new ProcessDocumentMessage { DocumentMetadata = documentMetadata });
                }
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