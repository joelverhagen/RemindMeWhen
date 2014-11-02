using System.Net.Http.Formatting;
using System.Web.Http;
using Knapcode.RemindMeWhen.WebApi.Formatters;
using Knapcode.StandardSerializer;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Knapcode.RemindMeWhen.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            configuration.Formatters.Clear();
            configuration.Formatters.Add(GetJsonMediaTypeFormatter());

            configuration.MapHttpAttributeRoutes();
            configuration.Routes.MapHttpRoute("MovieReleasedToHomeEvents_GetEvents", "events/movie-released-to-home", new {controller = "MovieReleasedToHomeEvents", action = "GetEvents"});
            configuration.Routes.MapHttpRoute("MovieReleasedToTheaterEvents_GetEvents", "events/movie-released-to-theater", new {controller = "MovieReleasedToTheaterEvents", action = "GetEvents"});
        }

        private static JsonMediaTypeFormatter GetJsonMediaTypeFormatter()
        {
            var resolver = new StandardContractResolver
            {
                CapitalizationOptions = CapitalizationOptions.AllLowercase,
                WordSplitOptions = WordSplitOptions.SplitCamelCase,
                WordDelimiter = "_"
            };

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = resolver
            };

            serializerSettings.Converters.Add(new StringEnumConverter());

            return new JsonpMediaTypeFormatter
            {
                UseDataContractJsonSerializer = false,
                SerializerSettings = serializerSettings
            };
        }
    }
}