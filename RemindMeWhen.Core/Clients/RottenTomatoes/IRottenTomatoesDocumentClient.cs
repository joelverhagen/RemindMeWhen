using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes
{
    public interface IRottenTomatoesDocumentClient
    {
        Task<Document> SearchMoviesAsync(string query, int page, int pageLimit);
    }
}