using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using Knapcode.RemindMeWhen.Core.Clients;
using Knapcode.RemindMeWhen.Core.Queue;
using Newtonsoft.Json;

namespace Knapcode.RemindMeWhen.Core.Logging
{
    public class EventSource : IEventSource
    {
        public void OnFetchedDocumentFromRottenTomatoesApi(DocumentId documentId, HttpStatusCode httpStatusCode, long length, TimeSpan duration)
        {
            Log(new {documentId, httpStatusCode, length, duration});
        }

        public void OnMissingRecordFromAzure(string partitionKey, string rowKey)
        {
            Log(new {partitionKey, rowKey});
        }

        public void OnFetchedRecordFromAzure(string partitionKey, string rowKey, TimeSpan duration)
        {
            Log(new {partitionKey, rowKey, duration});
        }

        public void OnSavedRecordToAzure(string partitionKey, string rowKey, TimeSpan duration)
        {
            Log(new {partitionKey, rowKey, duration});
        }

        public void OnFetchedListFromAzure(string partitionKey, int count, TimeSpan duration)
        {
            Log(new {partitionKey, count, duration});
        }

        public void OnMissingBlobFromAzure(string key)
        {
            Log(new {key});
        }

        public void OnBlobDownloadFromAzure(string key, long length, TimeSpan duration)
        {
            Log(new {key, length, duration});
        }

        public void OnBlobUploadedToAzure(string key, long length, TimeSpan duration)
        {
            Log(new {key, length, duration});
        }

        public void OnBlobExistenceCheckedInAzure(string key, bool exists, TimeSpan duration)
        {
            Log(new {key, exists, duration});
        }

        public void OnMissingDocumentMetadataFromDocumentStore(DocumentId documentId, string documentMetadataId)
        {
            Log(new {documentId, documentMetadataId});
        }

        public void OnMissingDocumentFromDocumentStore(DocumentMetadata documentMetadata)
        {
            Log(new {documentMetadata});
        }

        public void OnDuplicateFoundInDocumentStore(DocumentId documentId, string documentHash)
        {
            Log(new {documentId, documentHash});
        }

        public void OnQueueMessagePeeked(TimeSpan duration)
        {
            Log(new {duration});
        }

        public void OnQueueMessageFetched(TimeSpan duration)
        {
            Log(new {duration});
        }

        public void OnQueueMessageDeleted(TimeSpan duration)
        {
            Log(new {duration});
        }

        public void OnQueueMessageUpdated(TimeSpan duration)
        {
            Log(new {duration});
        }

        public void OnQueueMessageAdded(TimeSpan duration)
        {
            Log(new {duration});
        }

        public void OnCompressed(long decompressedLength, long compressedLength, TimeSpan duration)
        {
            Log(new {decompressedLength, compressedLength, duration});
        }

        public void OnDecompressed(long compressedLength, long decompressedLength, TimeSpan duration)
        {
            Log(new {compressedLength, decompressedLength, duration});
        }

        public void OnCompletedProcessDocumentJobDueToMissingDocument(ProcessDocumentMessage processDocumentMessage)
        {
            Log(new {processDocumentMessage});
        }

        public void OnMissingSubscriptionFromSubscriptionStore(Guid userId, string uniqueId)
        {
            Log(new {userId, uniqueId});
        }

        private static void Log(object obj, [CallerMemberName] string callerMemberName = null)
        {
            Trace.WriteLine(JsonConvert.SerializeObject(new {Content = obj, CallerMemberName = callerMemberName}));
        }
    }
}