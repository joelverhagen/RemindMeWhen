using System.Text;
using Newtonsoft.Json;

namespace Knapcode.RemindMeWhen.Core.Clients
{
    public class JsonDeserializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public JsonDeserializer(JsonSerializerSettings jsonSerializerSettings)
        {
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        protected T Deserialize<T>(byte[] buffer)
        {
            string jsonString = Encoding.UTF8.GetString(buffer);
            return JsonConvert.DeserializeObject<T>(jsonString, _jsonSerializerSettings);
        }
    }
}