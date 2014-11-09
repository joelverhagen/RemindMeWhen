using System;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Knapcode.RemindMeWhen.Core.Queue
{
    public class AzureQueue<T> : IQueue<T>
    {
        private readonly CloudQueue _cloudQueue;
        private readonly IEventSource _eventSource;

        public AzureQueue(IEventSource eventSource, CloudQueue cloudQueue)
        {
            _eventSource = eventSource;
            _cloudQueue = cloudQueue;
        }

        public async Task<QueueMessage<T>> PeekMessageAsync()
        {
            CloudQueueMessage cloudQueueMessage;
            using (EventTimer.OnCompletion(d => _eventSource.OnQueueMessagePeeked(d)))
            {
                cloudQueueMessage = await _cloudQueue.PeekMessageAsync();
            }
            return Deserialize(cloudQueueMessage);
        }

        public async Task<QueueMessage<T>> GetMessageAsync()
        {
            CloudQueueMessage cloudQueueMessage;
            using (EventTimer.OnCompletion(d => _eventSource.OnQueueMessageFetched(d)))
            {
                cloudQueueMessage = await _cloudQueue.GetMessageAsync();
            }
            return Deserialize(cloudQueueMessage);
        }

        public async Task DeleteMessageAsync(QueueMessage<T> queueMessage)
        {
            using (EventTimer.OnCompletion(d => _eventSource.OnQueueMessageDeleted(d)))
            {
                await _cloudQueue.DeleteMessageAsync(queueMessage.Id, queueMessage.PopReceipt);
            }
        }

        public async Task UpdateMessageAsync(QueueMessage<T> queueMessage, TimeSpan visibilityTimeout)
        {
            var cloudQueueMessage = new CloudQueueMessage(queueMessage.Id, queueMessage.PopReceipt);

            string serializedContent = Serialize(queueMessage.Content);
            cloudQueueMessage.SetMessageContent(serializedContent);

            using (EventTimer.OnCompletion(d => _eventSource.OnQueueMessageUpdated(d)))
            {
                await _cloudQueue.UpdateMessageAsync(cloudQueueMessage, visibilityTimeout, MessageUpdateFields.Content | MessageUpdateFields.Visibility);
            }
        }

        public async Task AddMessageAsync(T content)
        {
            string serializedContent = Serialize(content);

            var cloudQueueMessage = new CloudQueueMessage(serializedContent);

            using (EventTimer.OnCompletion(d => _eventSource.OnQueueMessageAdded(d)))
            {
                await _cloudQueue.AddMessageAsync(cloudQueueMessage);
            }
        }

        private static string Serialize(T deserializedContent)
        {
            return JsonConvert.SerializeObject(deserializedContent);
        }

        private static QueueMessage<T> Deserialize(CloudQueueMessage cloudQueueMessage)
        {
            var deserializedContent = JsonConvert.DeserializeObject<T>(cloudQueueMessage.AsString);
            return new QueueMessage<T>
            {
                Content = deserializedContent,
                Id = cloudQueueMessage.Id,
                PopReceipt = cloudQueueMessage.PopReceipt
            };
        }
    }
}