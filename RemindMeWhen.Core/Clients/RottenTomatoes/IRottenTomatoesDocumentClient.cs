using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Clients.RottenTomatoes
{
    public interface IRottenTomatoesDocumentClient
    {
        DocumentId SearchMovies(string query, PageOffset pageOffset);
        Task<Document> GetDocumentAsync(DocumentId documentId);
    }
}