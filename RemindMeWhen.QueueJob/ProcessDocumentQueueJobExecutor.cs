using System;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Knapcode.RemindMeWhen.QueueJob
{
    public static class ProcessDocumentQueueJobExecutor
    {
        public static Func<ProcessDocumentJob> Initializer { get; set; }

        public static async Task ExecuteWithNewJobAsync([QueueTrigger("%" + NameResolver.ProcessDocumentQueueNameKey + "%")] CloudQueueMessage cloudQueueMessage)
        {
            ProcessDocumentJob processDocumentJob = Initializer();
            QueueMessage<ProcessDocumentMessage> queueMessage = AzureQueue<ProcessDocumentMessage>.Deserialize(cloudQueueMessage);
            await processDocumentJob.ExecuteAsync(queueMessage.Content);
        }
    }
}