using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Knapcode.RemindMeWhen.Core.Queue
{
    public class AzureQueue<T> : IQueue<T>
    {
        private readonly CloudQueue _cloudQueue;

        public AzureQueue(CloudQueue cloudQueue)
        {
            _cloudQueue = cloudQueue;
        }

        public async Task<QueueMessage<T>> GetMessageAsync()
        {
            CloudQueueMessage cloudQueueMessage = await _cloudQueue.GetMessageAsync();

            T deserializedContent = Deserialize(cloudQueueMessage.AsString);

            return new QueueMessage<T>
            {
                Content = deserializedContent,
                Id = cloudQueueMessage.Id,
                PopReceipt = cloudQueueMessage.PopReceipt
            };
        }

        public async Task DeleteMessageAsync(QueueMessage<T> queueMessage)
        {
            await _cloudQueue.DeleteMessageAsync(queueMessage.Id, queueMessage.PopReceipt);
        }

        public async Task UpdateMessageAsync(QueueMessage<T> queueMessage, TimeSpan visibilityTimeout)
        {
            var cloudQueueMessage = new CloudQueueMessage(queueMessage.Id, queueMessage.PopReceipt);

            string serializedContent = Serialize(queueMessage.Content);
            cloudQueueMessage.SetMessageContent(serializedContent);

            await _cloudQueue.UpdateMessageAsync(cloudQueueMessage, visibilityTimeout, MessageUpdateFields.Content | MessageUpdateFields.Visibility);
        }

        public async Task AddMessageAsync(T content)
        {
            string serializedContent = Serialize(content);

            var cloudQueueMessage = new CloudQueueMessage(serializedContent);

            await _cloudQueue.AddMessageAsync(cloudQueueMessage);
        }

        private static string Serialize(T deserializedContent)
        {
            return JsonConvert.SerializeObject(deserializedContent);
        }

        private static T Deserialize(string serializedContent)
        {
            return JsonConvert.DeserializeObject<T>(serializedContent);
        }
    }
}