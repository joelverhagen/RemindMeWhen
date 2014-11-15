using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Knapcode.RemindMeWhen.Core.Logging;
using Knapcode.RemindMeWhen.Core.Models;
using Knapcode.RemindMeWhen.Core.Settings;
using Knapcode.RemindMeWhen.Core.Support;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes
{
    public class RottenTomatoesDocumentClient : IRottenTomatoesDocumentClient
    {
        private readonly IEventSource _eventSource;
        private readonly HttpClient _httpClient;
        private readonly string _key;

        public RottenTomatoesDocumentClient(IEventSource eventSource, RottenTomatoesSettings settings)
        {
            Guard.ArgumentNotNull(settings, "settings");

            if (string.IsNullOrWhiteSpace(settings.ApiKey))
            {
                throw new ArgumentException("The Rotten Tomatoes API key cannot be null or just whitespace.", "settings");
            }

            _eventSource = eventSource;
            _key = settings.ApiKey;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://api.rottentomatoes.com/api/public/v1.0/")
            };
        }

        public async Task<Document> SearchMoviesAsync(string query, PageOffset pageOffset)
        {
            Guard.ArgumentNotNull(query, "query");
            Guard.ArgumentNotNull(pageOffset, "pageOffset");

            var parameters = new Dictionary<string, string>
            {
                {"q", query},
                {"page_limit", pageOffset.Size.ToString(CultureInfo.InvariantCulture)},
                {"page", (pageOffset.Index + 1).ToString(CultureInfo.InvariantCulture)}
            };

            string requestUri = GetRequestUri("movies.json", parameters);

            using (EventTimer.OnCompletion(d => _eventSource.OnSearchedRottenTomatoesForMovies(query, pageOffset, d)))
            {
                HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
                return await GetExternalDocumentAsync(response);
            }
        }

        private async Task<Document> GetExternalDocumentAsync(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            string typeId = response.RequestMessage.RequestUri.ToString().Replace(_key, "key");
            byte[] content = await response.Content.ReadAsByteArrayAsync();
            return new Document
            {
                Id = new DocumentId
                {
                    Type = DocumentType.RottenTomatoesApiMovieSearch,
                    TypeId = typeId
                },
                Content = content
            };
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