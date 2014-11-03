using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models;
using Knapcode.RemindMeWhen.Core.Settings;
using Knapcode.StandardSerializer;
using Newtonsoft.Json;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes
{
    public class RottenTomatoesClient : IRottenTomatoesClient
    {
        private static readonly List<MediaTypeFormatter> MediaTypeFormatters = new List<MediaTypeFormatter>();

        private readonly HttpClient _httpClient;
        private readonly string _key;

        static RottenTomatoesClient()
        {
            var resolver = new StandardContractResolver
            {
                WordSplitOptions = WordSplitOptions.SplitCamelCase,
                CapitalizationOptions = CapitalizationOptions.AllLowercase,
                WordDelimiter = "_"
            };

            var jsonFormatter = new JsonMediaTypeFormatter
            {
                SerializerSettings = new JsonSerializerSettings {ContractResolver = resolver},
                UseDataContractJsonSerializer = false
            };
            jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));

            MediaTypeFormatters.Add(jsonFormatter);
        }

        public RottenTomatoesClient(RottenTomatoesClientSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (string.IsNullOrWhiteSpace(settings.Key))
            {
                throw new ArgumentException("The Rotten Tomatoes API key cannot be null.", "settings");
            }

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

            return await response.Content.ReadAsAsync<MovieCollection>(MediaTypeFormatters);
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