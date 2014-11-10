﻿using System.Text;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models;
using Knapcode.StandardSerializer;
using Newtonsoft.Json;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes
{
    public class RottenTomatoesDeserializer : IDeserializer<MovieCollection>
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

        public MovieCollection Deserialize(byte[] content)
        {
            return Deserialize<MovieCollection>(content);
        }

        private static T Deserialize<T>(byte[] content)
        {
            string json = Encoding.UTF8.GetString(content);
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }
    }
}