using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Repositories
{
    public interface IMovieReleaseRepository
    {
        Task<Page<MovieRelease>> GetMovieReleasesPage(string query, int pageLimit, int pageNumber);
    }
}