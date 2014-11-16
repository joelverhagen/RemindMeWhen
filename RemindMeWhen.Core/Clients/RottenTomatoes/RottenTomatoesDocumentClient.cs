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
            _httpClient = new HttpClient();
        }

        public DocumentId SearchMovies(string query, PageOffset pageOffset)
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

            return new DocumentId
            {
                Type = DocumentType.RottenTomatoesApiMovieSearch,
                TypeId = requestUri
            };
        }

        public async Task<Document> GetDocumentAsync(DocumentId documentId)
        {
            Guard.ArgumentNotNull(documentId, "documentId");
            return await GetExternalDocumentAsync(documentId);
        }

        private async Task<Document> GetExternalDocumentAsync(DocumentId documentId)
        {
            // build the request URI
            var builder = new UriBuilder(documentId.TypeId);
            NameValueCollection queryString = HttpUtility.ParseQueryString(builder.Query);
            queryString["apiKey"] = _key;
            builder.Query = queryString.ToString();
            string requestUri = builder.ToString();

            // make the HTTP request
            HttpResponseMessage response = null;
            byte[] content = null;
            TimeSpan duration = await EventTimer.TimeAsync(async () =>
            {
                response = await _httpClient.GetAsync(requestUri);
                content = await response.Content.ReadAsByteArrayAsync();
            });
            _eventSource.OnFetchedDocumentFromRottenTomatoesApi(documentId, response.StatusCode, content.LongLength, duration);

            response.EnsureSuccessStatusCode();

            return new Document
            {
                Id = documentId,
                Content = content
            };
        }

        private string GetRequestUri(string path, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
            foreach (var pair in parameters)
            {
                queryString[pair.Key] = pair.Value;
            }

            var builder = new UriBuilder
            {
                Scheme = "http",
                Host = "api.rottentomatoes.com",
                Path = "/api/public/v1.0/" + path,
                Query = queryString.ToString()
            };

            return builder.ToString();
        }
    }
}