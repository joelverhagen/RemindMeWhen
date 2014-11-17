using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Queue
{
    public interface IQueueJob<in T>
    {
        Task ExecuteAsync(T message);
    }
}