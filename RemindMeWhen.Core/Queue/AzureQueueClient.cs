using Knapcode.RemindMeWhen.Core.Logging;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Knapcode.RemindMeWhen.Core.Queue
{
    public class AzureQueueClient : IQueueClient
    {
        private readonly CloudQueueClient _cloudQueueClient;
        private readonly IEventSource _eventSource;

        public AzureQueueClient(IEventSource eventSource, CloudQueueClient cloudQueueClient)
        {
            _eventSource = eventSource;
            _cloudQueueClient = cloudQueueClient;
        }

        public IQueue<T> GetQueue<T>(string queueName)
        {
            return new AzureQueue<T>(_eventSource, _cloudQueueClient.GetQueueReference(queueName));
        }
    }
}