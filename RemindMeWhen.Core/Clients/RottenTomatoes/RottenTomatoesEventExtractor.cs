using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes
{
    public class RottenTomatoesEventExtractor : IEventExtractor<byte[], MovieReleasedToTheaterEvent>, IEventExtractor<byte[], MovieReleasedToHomeEvent>, IEventExtractor<byte[], MovieReleasedEvent>
    {
        private static readonly IDictionary<Type, Tuple<EventType, Func<ReleaseDates, DateTime?>>> EventTypeMap = new Dictionary<Type, Tuple<EventType, Func<ReleaseDates, DateTime?>>>
        {
            {
                typeof (MovieReleasedToTheaterEvent),
                new Tuple<EventType, Func<ReleaseDates, DateTime?>>(EventType.MovieReleasedToTheater, r => r.Theater)
            },
            {
                typeof (MovieReleasedToHomeEvent),
                new Tuple<EventType, Func<ReleaseDates, DateTime?>>(EventType.MovieReleasedToHome, r => r.Dvd)
            }
        };

        private readonly IDeserializer<MovieCollection> _deserializer;

        public RottenTomatoesEventExtractor(IDeserializer<MovieCollection> deserializer)
        {
            _deserializer = deserializer;
        }

        public IEnumerable<MovieReleasedEvent> Extract(byte[] content)
        {
            return Enumerable
                .Empty<MovieReleasedEvent>()
                .Concat(GetMovieReleaseEvents<MovieReleasedToTheaterEvent>(content))
                .Concat(GetMovieReleaseEvents<MovieReleasedToHomeEvent>(content));
        }

        IEnumerable<MovieReleasedToHomeEvent> IEventExtractor<byte[], MovieReleasedToHomeEvent>.Extract(byte[] content)
        {
            return GetMovieReleaseEvents<MovieReleasedToHomeEvent>(content);
        }

        IEnumerable<MovieReleasedToTheaterEvent> IEventExtractor<byte[], MovieReleasedToTheaterEvent>.Extract(byte[] content)
        {
            return GetMovieReleaseEvents<MovieReleasedToTheaterEvent>(content);
        }

        private IEnumerable<T> GetMovieReleaseEvents<T>(byte[] content) where T : MovieReleasedEvent
        {
            MovieCollection movieCollection = _deserializer.Deserialize(content);

            if (movieCollection == null || movieCollection.Movies == null)
            {
                yield break;
            }

            foreach (Movie movie in movieCollection.Movies)
            {
                yield return GetMovieReleaseEvent<T>(movie);
            }
        }

        private static T GetMovieReleaseEvent<T>(Movie movie) where T : MovieReleasedEvent
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
            string[] actors;
            if (movie.AbridgedCast != null)
            {
                actors = movie
                    .AbridgedCast
                    .Where(a => a != null && a.Name != null)
                    .Select(a => a.Name)
                    .ToArray();
            }
            else
            {
                actors = new string[0];
            }

            // get the poster, but ignore default images
            Uri imageUrl = movie.Posters != null ? movie.Posters.Original ?? movie.Posters.Detailed ?? movie.Posters.Profile ?? movie.Posters.Thumbnail : null;
            if (imageUrl != null && imageUrl.ToString().Contains("cloudfront.net/static/images/redesign/poster_default_thumb.gif"))
            {
                imageUrl = null;
            }

            // build the event content
            var content = new MovieReleasedEventContent
            {
                ImdbUrl = imdbUrl,
                Title = movie.Title,
                RottenTomatoesUrl = rottenTomatoesUrl,
                ReleasedYear = movie.Year,
                Actors = actors,
                ImageUrl = imageUrl
            };

            // get the event date and type
            Type type = typeof (T);
            Tuple<EventType, Func<ReleaseDates, DateTime?>> tuple;
            if (!EventTypeMap.TryGetValue(type, out tuple))
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "The provided type parameter '{0}' is not valid.",
                    type.FullName);
                throw new ArgumentException(message);
            }

            EventType eventType = tuple.Item1;
            DateTime? dateTime = movie.ReleaseDates != null ? tuple.Item2(movie.ReleaseDates) : null;
            var e = Activator.CreateInstance<T>();
            e.Content = content;
            e.DateTime = dateTime;
            e.Id = new EventId
            {
                ProviderId = ProviderId.RottenTomatoesApi,
                Type = eventType,
                ResourceId = movie.Id
            };
            return e;
        }
    }
}