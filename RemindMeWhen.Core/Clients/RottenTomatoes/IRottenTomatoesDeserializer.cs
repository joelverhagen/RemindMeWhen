using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes
{
    public interface IRottenTomatoesDeserializer
    {
        MovieCollection DeserializeMovieCollection(byte[] buffer);
        Movie DeserializeMovie(byte[] buffer);
    }
}