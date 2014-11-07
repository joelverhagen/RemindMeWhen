using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Queue
{
    public interface IQueueClient
    {
        Task<IQueue<T>> GetQueueAsync<T>();
    }
}