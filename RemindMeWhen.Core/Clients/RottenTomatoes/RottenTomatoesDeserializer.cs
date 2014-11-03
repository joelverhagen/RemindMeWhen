using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models;
using Knapcode.StandardSerializer;
using Newtonsoft.Json;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes
{
    public class RottenTomatoesDeserializer : JsonDeserializer, IDeserializer<MovieCollection>, IDeserializer<Movie>, IRottenTomatoesDeserializer
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings;

        static RottenTomatoesDeserializer()
        {
            var resolver = new StandardContractResolver
            {
                WordSplitOptions = WordSplitOptions.SplitCamelCase,
                CapitalizationOptions = CapitalizationOptions.AllLowercase,
                WordDelimiter = "_"
            };

            JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = resolver
            };
        }

        public RottenTomatoesDeserializer() : base(JsonSerializerSettings)
        {
        }

        Movie IDeserializer<Movie>.Deserialize(byte[] buffer)
        {
            return DeserializeMovie(buffer);
        }

        MovieCollection IDeserializer<MovieCollection>.Deserialize(byte[] buffer)
        {
            return DeserializeMovieCollection(buffer);
        }

        public MovieCollection DeserializeMovieCollection(byte[] buffer)
        {
            return Deserialize<MovieCollection>(buffer);
        }

        public Movie DeserializeMovie(byte[] buffer)
        {
            return Deserialize<Movie>(buffer);
        }
    }
}