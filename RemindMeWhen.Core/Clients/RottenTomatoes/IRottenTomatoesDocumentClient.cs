using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes
{
    public interface IRottenTomatoesDocumentClient
    {
        Task<ExternalDocument> SearchMoviesAsync(string query, int page, int pageLimit);
    }
}