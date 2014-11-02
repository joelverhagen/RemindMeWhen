using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Knapcode.RemindMeWhen.WebApi.Formatters
{
    public class JsonpMediaTypeFormatter : JsonMediaTypeFormatter
    {
        private readonly HttpRequestMessage _request;

        public JsonpMediaTypeFormatter() : this(null)
        {
        }

        private JsonpMediaTypeFormatter(HttpRequestMessage request)
        {
            CallbackQueryKey = "callback";
            _request = request;
        }

        public string CallbackQueryKey { get; set; }

        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            return new JsonpMediaTypeFormatter(request)
            {
                Indent = Indent,
                UseDataContractJsonSerializer = UseDataContractJsonSerializer,
                SerializerSettings = SerializerSettings,
                RequiredMemberSelector = RequiredMemberSelector,
                MaxDepth = MaxDepth
            };
        }

        public override async Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext, CancellationToken cancellationToken)
        {
            string callback = GetJsonpCallback();

            if (callback == null)
            {
                await base.WriteToStreamAsync(type, value, writeStream, content, transportContext, cancellationToken);
                return;
            }

            var encoding = SelectCharacterEncoding(content == null ? null : content.Headers);
            var writer = new StreamWriter(writeStream, encoding);
            await writer.WriteAsync(callback);
            await writer.WriteAsync("(");
            await writer.FlushAsync();
            await base.WriteToStreamAsync(type, value, writeStream, content, transportContext, cancellationToken);
            await writer.WriteAsync(");");
            await writer.FlushAsync();
        }

        private string GetJsonpCallback()
        {
            if (_request == null)
            {
                return null;
            }

            NameValueCollection queryString = HttpUtility.ParseQueryString(_request.RequestUri.Query);
            string callback = queryString[CallbackQueryKey];
            if (string.IsNullOrWhiteSpace(callback))
            {
                callback = null;
            }

            return callback;
        }
    }
}