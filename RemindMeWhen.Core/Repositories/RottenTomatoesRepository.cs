using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Repositories
{
    public class RottenTomatoesRepository : IEventSearchRepository<MovieReleasedToTheaterEvent>, IEventSearchRepository<MovieReleasedToHomeEvent>, IRottenTomatoesRepository
    {
        private const string Source = "api.rottentomatoes.com";

        private static readonly IDictionary<EventType, Func<ReleaseDates, DateTime?>> DateGetters = new Dictionary<EventType, Func<ReleaseDates, DateTime?>>
        {
            {EventType.MovieReleasedToHome, r => r.Dvd},
            {EventType.MovieReleasedToTheater, r => r.Theater},
        };

        private readonly IRottenTomatoesClient _client;

        public RottenTomatoesRepository(IRottenTomatoesClient client)
        {
            _client = client;
        }

        async Task<Page<MovieReleasedToHomeEvent>> IEventSearchRepository<MovieReleasedToHomeEvent>.SearchEventsAsync(string query, int pageLimit, int pageNumber)
        {
            return await SearchMovieReleaseToHomeEventsAsync(query, pageLimit, pageNumber);
        }

        async Task<Page<MovieReleasedToTheaterEvent>> IEventSearchRepository<MovieReleasedToTheaterEvent>.SearchEventsAsync(string query, int pageLimit, int pageNumber)
        {
            return await SearchMovieReleaseToTheaterEventsAsync(query, pageLimit, pageNumber);
        }

        public async Task<Page<MovieReleasedToHomeEvent>> SearchMovieReleaseToHomeEventsAsync(string query, int pageLimit, int pageNumber)
        {
            return await GetMovieReleasePageAsync<MovieReleasedToHomeEvent>(EventType.MovieReleasedToHome, query, pageLimit, pageNumber);
        }

        public async Task<Page<MovieReleasedToTheaterEvent>> SearchMovieReleaseToTheaterEventsAsync(string query, int pageLimit, int pageNumber)
        {
            return await GetMovieReleasePageAsync<MovieReleasedToTheaterEvent>(EventType.MovieReleasedToTheater, query, pageLimit, pageNumber);
        }

        private async Task<Page<T>> GetMovieReleasePageAsync<T>(EventType eventType, string query, int pageLimit, int pageNumber) where T : MovieReleasedEvent
        {
            Func<ReleaseDates, DateTime?> getDate;
            if (!DateGetters.TryGetValue(eventType, out getDate))
            {
                string message = string.Format("The event type '{0}' is not supported for a movie release event.", eventType);
                throw new ArgumentException(message, "eventType");
            }

            // query Rotten Tomatoes API
            MovieCollection movieCollection = await _client.SearchMoviesAsync(query, pageLimit, pageNumber);

            // mutate the result
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

                hasNextPage = movieCollection.Movies.Count() >= pageLimit;
            }

            return new Page<T>
            {
                PageLimit = pageLimit,
                PageNumber = pageNumber,
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