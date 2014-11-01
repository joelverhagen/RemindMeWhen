using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes.Models;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes
{
    public interface IRottenTomatoesClient
    {
        Task<MovieCollection> SearchMoviesAsync(string query, int pageLimit = 30, int page = 1);
    }
}