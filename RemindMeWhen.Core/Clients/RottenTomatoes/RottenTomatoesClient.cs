using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models;
using Knapcode.RemindMeWhen.Core.Settings;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes
{
    public class RottenTomatoesClient : IRottenTomatoesClient
    {
        private readonly IRottenTomatoesDeserializer _deserializer;
        private readonly HttpClient _httpClient;
        private readonly string _key;

        public RottenTomatoesClient(IRottenTomatoesDeserializer deserializer, RottenTomatoesClientSettings settings)
        {
            if (deserializer == null)
            {
                throw new ArgumentNullException("deserializer");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (string.IsNullOrWhiteSpace(settings.Key))
            {
                throw new ArgumentException("The Rotten Tomatoes API key cannot be null.", "settings");
            }

            _deserializer = deserializer;
            _key = settings.Key;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://api.rottentomatoes.com/api/public/v1.0/")
            };
        }

        public async Task<MovieCollection> SearchMoviesAsync(string query, int pageLimit = 30, int page = 1)
        {
            var parameters = new Dictionary<string, string>
            {
                {"q", query},
                {"page_limit", pageLimit.ToString(CultureInfo.InvariantCulture)},
                {"page", page.ToString(CultureInfo.InvariantCulture)}
            };

            string requestUri = GetRequestUri("movies.json", parameters);

            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
            byte[] buffer = await response.Content.ReadAsByteArrayAsync();
            return _deserializer.DeserializeMovieCollection(buffer);
        }

        private string GetRequestUri(string path, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);

            queryString["apikey"] = _key;

            foreach (var pair in parameters)
            {
                queryString[pair.Key] = pair.Value;
            }

            return path + "?" + queryString;
        }
    }
}