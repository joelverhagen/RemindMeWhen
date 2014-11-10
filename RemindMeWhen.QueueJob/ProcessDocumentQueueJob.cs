using Microsoft.Azure.WebJobs;

namespace Knapcode.RemindMeWhen.QueueJob
{
    public class ProcessDocumentQueueJob
    {
        public static void Execute([QueueTrigger("%" + NameResolver.ProcessDocumentQueueNameKey + "%")] string message)
        {
        }
    }
}