using System;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Knapcode.RemindMeWhen.QueueJob
{
    public static class ProcessDocumentQueueJobExecutor
    {
        public static Func<ProcessDocumentQueueJob> Initializer { get; set; }

        public static async Task ExecuteWithNewJobAsync([QueueTrigger("%" + NameResolver.ProcessDocumentQueueNameKey + "%")] CloudQueueMessage cloudQueueMessage)
        {
            ProcessDocumentQueueJob processDocumentQueueJob = Initializer();
            QueueMessage<ProcessDocument> queueMessage = AzureQueue<ProcessDocument>.Deserialize(cloudQueueMessage);
            await processDocumentQueueJob.ExecuteAsync(queueMessage);
        }
    }
}