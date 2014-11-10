using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Repositories
{
    public interface IRottenTomatoesRepository<T> where T : MovieReleasedEvent
    {
        Task<Page<T>> SearchMovieReleaseEventsAsync(string query, PageOffset pageOffset);
    }
}