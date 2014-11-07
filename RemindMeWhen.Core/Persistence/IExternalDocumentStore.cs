using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public interface IExternalDocumentStore
    {
        Task<ExternalDocument> GetAsync(string identity);
        Task SetAsync(ExternalDocument document);
    }
}