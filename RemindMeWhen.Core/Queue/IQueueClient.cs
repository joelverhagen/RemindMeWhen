using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Queue
{
    public interface IQueueClient
    {
        IQueue<T> GetQueue<T>(string queueName);
    }
}