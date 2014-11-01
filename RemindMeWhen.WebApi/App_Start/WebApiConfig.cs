using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.SessionState;
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
            configuration.Routes.MapHttpRoute("DefaultApi", "{controller}/{id}", new {id = RouteParameter.Optional});
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

            return new JsonMediaTypeFormatter
            {
                UseDataContractJsonSerializer = false,
                SerializerSettings = serializerSettings
            };
        }
    }
}