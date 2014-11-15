using System;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Clients;
using Knapcode.RemindMeWhen.Core.Logging;
using Knapcode.RemindMeWhen.Core.Persistence;
using Knapcode.RemindMeWhen.Core.Queue;

namespace Knapcode.RemindMeWhen.QueueJob
{
    public class ProcessDocumentQueueJob
    {
        private readonly IEventSource _eventSource;
        private readonly IDocumentStore _documentStore;

        public ProcessDocumentQueueJob(IEventSource eventSource, IDocumentStore documentStore)
        {
            _eventSource = eventSource;
            _documentStore = documentStore;
        }

        public static Func<ProcessDocumentQueueJob> Initializer { get; set; }

        public async Task ExecuteAsync(QueueMessage<ProcessDocument> queueMessage)
        {
            Document document = await _documentStore.GetDocumentAsync(queueMessage.Content.DocumentMetadata.Id);
            if (document == null)
            {
                _eventSource.OnCompletedProcessDocumentDueToMissingDocument(queueMessage);
                return;
            }


        }
    }
}