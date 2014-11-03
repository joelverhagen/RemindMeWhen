using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Models;

namespace Knapcode.RemindMeWhen.Core.Repositories
{
    public interface IEventSearchRepository<T> where T : IEvent
    {
        Task<Page<T>> SearchEventsAsync(string query, int pageLimit, int pageNumber);
    }
}