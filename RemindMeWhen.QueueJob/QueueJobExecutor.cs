using System;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Knapcode.RemindMeWhen.QueueJob
{
    public static class QueueJobExecutor
    {
        public static Func<Type, object> Resolver { get; set; }

        public static async Task ProcessDocumentAsync([QueueTrigger("%" + NameResolver.ProcessDocumentQueueNameKey + "%")] CloudQueueMessage cloudQueueMessage)
        {
            var job = (IQueueJob<ProcessDocumentMessage>) Resolver(typeof (IQueueJob<ProcessDocumentMessage>));
            QueueMessage<ProcessDocumentMessage> queueMessage = AzureQueue<ProcessDocumentMessage>.Deserialize(cloudQueueMessage);
            await job.ExecuteAsync(queueMessage.Content);
        }

        public static async Task SaveSubscriptionAsync([QueueTrigger("%" + NameResolver.SaveSubscriptionQueueNameKey + "%")] CloudQueueMessage cloudQueueMessage)
        {
            var job = (IQueueJob<SaveSubscriptionMessage>)Resolver(typeof(IQueueJob<SaveSubscriptionMessage>));
            QueueMessage<SaveSubscriptionMessage> queueMessage = AzureQueue<SaveSubscriptionMessage>.Deserialize(cloudQueueMessage);
            await job.ExecuteAsync(queueMessage.Content);
        }
    }
}