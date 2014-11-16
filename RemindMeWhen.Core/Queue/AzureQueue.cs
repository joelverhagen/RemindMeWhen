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
            CloudQueueMessage cloudQueueMessage = null;
            TimeSpan duration = await EventTimer.TimeAsync(async () =>
            {
                cloudQueueMessage = await _cloudQueue.PeekMessageAsync();
            });
            _eventSource.OnQueueMessagePeeked(duration);
            return Deserialize(cloudQueueMessage);
        }

        public async Task<QueueMessage<T>> GetMessageAsync()
        {
            CloudQueueMessage cloudQueueMessage = null;
            TimeSpan duration = await EventTimer.TimeAsync(async () =>
            {
                cloudQueueMessage = await _cloudQueue.GetMessageAsync();
            });
            _eventSource.OnQueueMessageFetched(duration);
            return Deserialize(cloudQueueMessage);
        }

        public async Task DeleteMessageAsync(QueueMessage<T> queueMessage)
        {
            TimeSpan duration = await EventTimer.TimeAsync(async () =>
            {
                await _cloudQueue.DeleteMessageAsync(queueMessage.Id, queueMessage.PopReceipt);
            });
            _eventSource.OnQueueMessageDeleted(duration);
        }

        public async Task UpdateMessageAsync(QueueMessage<T> queueMessage, TimeSpan visibilityTimeout)
        {
            var cloudQueueMessage = new CloudQueueMessage(queueMessage.Id, queueMessage.PopReceipt);

            string serializedContent = Serialize(queueMessage.Content);
            cloudQueueMessage.SetMessageContent(serializedContent);

            TimeSpan duration = await EventTimer.TimeAsync(async () =>
            {
                await _cloudQueue.UpdateMessageAsync(cloudQueueMessage, visibilityTimeout, MessageUpdateFields.Content | MessageUpdateFields.Visibility);
            });
            _eventSource.OnQueueMessageUpdated(duration);
        }

        public async Task AddMessageAsync(T content)
        {
            string serializedContent = Serialize(content);

            var cloudQueueMessage = new CloudQueueMessage(serializedContent);

            TimeSpan duration = await EventTimer.TimeAsync(async () =>
            {
                await _cloudQueue.AddMessageAsync(cloudQueueMessage);
            });
            _eventSource.OnQueueMessageAdded(duration);
        }

        private static string Serialize(T deserializedContent)
        {
            return JsonConvert.SerializeObject(deserializedContent);
        }

        public static QueueMessage<T> Deserialize(CloudQueueMessage cloudQueueMessage)
        {
            var content = JsonConvert.DeserializeObject<T>(cloudQueueMessage.AsString);
            return new QueueMessage<T>
            {
                Content = content,
                Id = cloudQueueMessage.Id,
                PopReceipt = cloudQueueMessage.PopReceipt
            };
        }
    }
}