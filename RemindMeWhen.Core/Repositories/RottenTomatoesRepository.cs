using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Repositories
{
    public class RottenTomatoesRepository : IEventSearchRepository<MovieReleasedToTheaterEvent>, IEventSearchRepository<MovieReleasedToHomeEvent>
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

        async Task<Page<MovieReleasedToHomeEvent>> IEventSearchRepository<MovieReleasedToHomeEvent>.EventSearchAsync(string query, int pageLimit, int pageNumber)
        {
            return await MovieReleaseToHomeEventsSearchAsync(query, pageLimit, pageNumber);
        }

        async Task<Page<MovieReleasedToTheaterEvent>> IEventSearchRepository<MovieReleasedToTheaterEvent>.EventSearchAsync(string query, int pageLimit, int pageNumber)
        {
            return await MovieReleaseToTheaterEventsSearchAsync(query, pageLimit, pageNumber);
        }

        public async Task<Page<MovieReleasedToHomeEvent>> MovieReleaseToHomeEventsSearchAsync(string query, int pageLimit, int pageNumber)
        {
            return await GetMovieReleasePageAsync<MovieReleasedToHomeEvent>(EventType.MovieReleasedToHome, query, pageLimit, pageNumber);
        }

        public async Task<Page<MovieReleasedToTheaterEvent>> MovieReleaseToTheaterEventsSearchAsync(string query, int pageLimit, int pageNumber)
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

            MovieCollection movieCollection = await _client.SearchMoviesAsync(query, pageLimit, pageNumber);

            var movieReleases = new List<T>();

            if (movieCollection != null && movieCollection.Movies != null)
            {
                foreach (Movie movie in movieCollection.Movies)
                {
                    if (movie.ReleaseDates == null)
                    {
                        continue;
                    }

                    DateTime? eventDate = getDate(movie.ReleaseDates);
                    if (!eventDate.HasValue)
                    {
                        continue;
                    }

                    movieReleases.Add(GetMovieReleaseEvent<T>(movie, eventType, eventDate.Value));
                }
            }

            // return the page
            return new Page<T>
            {
                Entries = movieReleases,
                PageLimit = pageLimit,
                PageNumber = pageNumber,
                HasNextPage = movieCollection.Movies.Count() >= pageLimit
            };
        }

        private static T GetMovieReleaseEvent<T>(Movie movie, EventType eventType, DateTime date) where T : MovieReleasedEvent
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
            e.ImdbUrl = imdbUrl;
            e.Title = movie.Title;
            e.RottenTomatoesUrl = rottenTomatoesUrl;
            e.ReleasedYear = movie.Year;
            e.Actors = actors;
            e.ImageUrl = imageUrl;

            return e;
        }
    }
}