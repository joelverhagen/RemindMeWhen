using System;
using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Queue
{
    public interface IQueue<T>
    {
        Task<QueueMessage<T>> GetMessageAsync();
        Task DeleteMessageAsync(QueueMessage<T> queueMessage);
        Task UpdateMessageAsync(QueueMessage<T> queueMessage, TimeSpan visibilityTimeout);
        Task AddMessageAsync(T content);
    }
}