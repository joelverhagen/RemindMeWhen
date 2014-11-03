using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Repositories
{
    public interface IRottenTomatoesRepository
    {
        Task<Page<MovieReleasedToHomeEvent>> SearchMovieReleaseToHomeEventsAsync(string query, int pageLimit, int pageNumber);
        Task<Page<MovieReleasedToTheaterEvent>> SearchMovieReleaseToTheaterEventsAsync(string query, int pageLimit, int pageNumber);
    }
}