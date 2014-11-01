using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Repositories
{
    public class RottenTomatoesMovieReleaseRepository : IMovieReleaseRepository
    {
        private readonly IRottenTomatoesClient _client;

        public RottenTomatoesMovieReleaseRepository(IRottenTomatoesClient client)
        {
            _client = client;
        }

        public async Task<Page<MovieRelease>> GetMovieReleasesPage(string query, int pageLimit, int pageNumber)
        {
            // cut the page limit in half because each Rotten Tomatoes movie can have two release dates (theater and home)
            pageLimit /= 2;

            // perform the search on Rotten Tomatoes
            MovieCollection movieCollection = await _client.SearchMoviesAsync(query, pageLimit, pageNumber);

            // extract theater and home releases
            var movieReleases = new List<MovieRelease>();
            foreach (Movie movie in movieCollection.Movies)
            {
                if (movie.ReleaseDates == null)
                {
                    continue;
                }

                if (movie.ReleaseDates.Theater.HasValue)
                {
                    movieReleases.Add(GetMovieRelease(movie, MovieReleaseType.Theater, movie.ReleaseDates.Theater.Value));
                }

                if (movie.ReleaseDates.Dvd.HasValue)
                {
                    movieReleases.Add(GetMovieRelease(movie, MovieReleaseType.Home, movie.ReleaseDates.Dvd.Value));
                }
            }

            // return the page
            return new Page<MovieRelease>
            {
                Entries = movieReleases,
                PageLimit = pageLimit,
                PageNumber = pageNumber,
                HasNextPage = movieCollection.Movies.Count() >= pageLimit
            };
        }

        private static MovieRelease GetMovieRelease(Movie movie, MovieReleaseType type, DateTime date)
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

            // get the poster
            Uri imageUrl = movie.Posters != null ? movie.Posters.Original ?? movie.Posters.Detailed ?? movie.Posters.Profile ?? movie.Posters.Thumbnail : null;

            // ignore Rotten Tomatoes' default image
            if (imageUrl != null && imageUrl.ToString().EndsWith("cloudfront.net/static/images/redesign/poster_default_thumb.gif"))
            {
                imageUrl = null;
            }

            return new MovieRelease(type)
            {
                Date = date,
                ImdbUrl = imdbUrl,
                Title = movie.Title,
                RottenTomatoesUrl = rottenTomatoesUrl,
                Year = movie.Year,
                Actors = actors,
                ImageUrl = imageUrl
            };
        }
    }
}
