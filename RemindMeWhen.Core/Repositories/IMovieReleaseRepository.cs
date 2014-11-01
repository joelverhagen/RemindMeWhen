using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Repositories
{
    public interface IMovieReleaseRepository
    {
        Task<Page<MovieReleaseEvent>> GetMovieReleasesPage(string query, int pageLimit, int pageNumber);
    }
}