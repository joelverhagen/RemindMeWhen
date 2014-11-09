using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models;
using Knapcode.RemindMeWhen.Core.Identities;
using Knapcode.RemindMeWhen.Core.Models;
using Knapcode.RemindMeWhen.Core.Persistence;
using Knapcode.RemindMeWhen.Core.Queue;

namespace Knapcode.RemindMeWhen.Core.Repositories
{
    public class RottenTomatoesRepository : IRottenTomatoesRepository
    {
        private const string Source = "api.rottentomatoes.com";

        private static readonly IDictionary<EventType, Func<ReleaseDates, DateTime?>> DateGetters = new Dictionary<EventType, Func<ReleaseDates, DateTime?>>
        {
            {EventType.MovieReleasedToHome, r => r.Dvd},
            {EventType.MovieReleasedToTheater, r => r.Theater},
        };

        private readonly IRottenTomatoesDeserializer _deserializer;
        private readonly IDocumentStore _documentStore;
        private readonly IRottenTomatoesDocumentClient _externalDocumentClient;
        private readonly IQueue<ProcessDocument> _queue;

        public RottenTomatoesRepository(IRottenTomatoesDocumentClient externalDocumentClient, IDocumentStore documentStore, IQueue<ProcessDocument> queue, IRottenTomatoesDeserializer deserializer)
        {
            _externalDocumentClient = externalDocumentClient;
            _documentStore = documentStore;
            _queue = queue;
            _deserializer = deserializer;
        }

        public async Task<Page<MovieReleasedToHomeEvent>> SearchMovieReleaseToHomeEventsAsync(string query, PageOffset pageOffset)
        {
            return await GetMovieReleasePageAsync<MovieReleasedToHomeEvent>(EventType.MovieReleasedToHome, query, pageOffset);
        }

        public async Task<Page<MovieReleasedToTheaterEvent>> SearchMovieReleaseToTheaterEventsAsync(string query, PageOffset pageOffset)
        {
            return await GetMovieReleasePageAsync<MovieReleasedToTheaterEvent>(EventType.MovieReleasedToTheater, query, pageOffset);
        }

        private async Task<Page<T>> GetMovieReleasePageAsync<T>(EventType eventType, string query, PageOffset pageOffset) where T : MovieReleasedEvent
        {
            Func<ReleaseDates, DateTime?> getDate;
            if (!DateGetters.TryGetValue(eventType, out getDate))
            {
                string message = string.Format("The event type '{0}' is not supported for a movie release event.", eventType);
                throw new ArgumentException(message, "eventType");
            }

            // query Rotten Tomatoes API
            Document document = await _externalDocumentClient.SearchMoviesAsync(query, pageOffset);

            // persist the document
            bool isDuplicate = await _documentStore.PersistUniqueDocumentAsync(document);

            // enqueue the process queue message if the document is new
            if (!isDuplicate)
            {
                await _queue.AddMessageAsync(new ProcessDocument {DocumentIdentity = document.Identity});
            }

            // mutate the result
            MovieCollection movieCollection = _deserializer.DeserializeMovieCollection(document.Content);
            var movieReleases = new List<T>();
            bool hasNextPage = false;
            if (movieCollection != null && movieCollection.Movies != null)
            {
                foreach (Movie movie in movieCollection.Movies)
                {
                    if (movie.ReleaseDates == null)
                    {
                        continue;
                    }

                    if (!movie.ReleaseDates.Theater.HasValue && !movie.ReleaseDates.Dvd.HasValue)
                    {
                        continue;
                    }

                    DateTime? date = getDate(movie.ReleaseDates);
                    movieReleases.Add(GetMovieReleaseEvent<T>(movie, eventType, date));
                }

                hasNextPage = movieCollection.Movies.Count() >= pageOffset.Size;
            }

            return new Page<T>
            {
                Offset = pageOffset,
                Entries = movieReleases,
                HasNextPage = hasNextPage
            };
        }

        private static T GetMovieReleaseEvent<T>(Movie movie, EventType eventType, DateTime? date) where T : MovieReleasedEvent
        {
            // get the IMDB URL
            Uri imdbUrl = null;
            if (movie.AlternateIds != null && movie.AlternateIds.Imdb != null)
            {
                imdbUrl = new Uri("http://www.imdb.com/title/tt" + movie.AlternateIds.Imdb);
            }

            // get the Rotten Tomatoes URL
            Uri rottenTomatoesUrl = null;
            if (movie.Links != null && movie.Links.Alternate != null)
            {
                rottenTomatoesUrl = movie.Links.Alternate;
            }

            // get the list of actors
            IEnumerable<string> actors;
            if (movie.AbridgedCast != null)
            {
                actors = movie.AbridgedCast.Where(a => a != null && a.Name != null).Select(a => a.Name);
            }
            else
            {
                actors = Enumerable.Empty<string>();
            }

            // get the poster, but ignore default images
            Uri imageUrl = movie.Posters != null ? movie.Posters.Original ?? movie.Posters.Detailed ?? movie.Posters.Profile ?? movie.Posters.Thumbnail : null;
            if (imageUrl != null && imageUrl.ToString().Contains("cloudfront.net/static/images/redesign/poster_default_thumb.gif"))
            {
                imageUrl = null;
            }

            var e = Activator.CreateInstance<T>();
            e.Identity = new EventIdentity(eventType, Source, movie.Id);
            e.DateTime = date;
            e.Content = new MovieReleasedEventContent
            {
                ImdbUrl = imdbUrl,
                Title = movie.Title,
                RottenTomatoesUrl = rottenTomatoesUrl,
                ReleasedYear = movie.Year,
                Actors = actors,
                ImageUrl = imageUrl
            };

            return e;
        }
    }
}