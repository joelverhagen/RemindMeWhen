using System;
using System.Net;
using Knapcode.RemindMeWhen.Core.Clients;
using Knapcode.RemindMeWhen.Core.Queue;

namespace Knapcode.RemindMeWhen.Core.Logging
{
    public interface IEventSource
    {
        void OnFetchedDocumentFromRottenTomatoesApi(DocumentId documentId, HttpStatusCode httpStatusCode, long length, TimeSpan duration);

        void OnMissingRecordFromAzure(string partitionKey, string rowKey);
        void OnFetchedRecordFromAzure(string partitionKey, string rowKey, TimeSpan duration);
        void OnSavedRecordToAzure(string partitionKey, string rowKey, TimeSpan duration);
        void OnFetchedListFromAzure(string partitionKey, int count, TimeSpan duration);

        void OnMissingBlobFromAzure(string key);
        void OnBlobDownloadFromAzure(string key, long length, TimeSpan duration);
        void OnBlobUploadedToAzure(string key, long length, TimeSpan duration);
        void OnBlobExistenceCheckedInAzure(string key, bool exists, TimeSpan duration);

        void OnMissingDocumentMetadataFromDocumentStore(DocumentId documentId, string documentMetadataId);
        void OnMissingDocumentFromDocumentStore(DocumentMetadata documentMetadata);
        void OnDuplicateFoundInDocumentStore(DocumentId documentId, string documentHash);
        
        void OnQueueMessagePeeked(TimeSpan duration);
        void OnQueueMessageFetched(TimeSpan duration);
        void OnQueueMessageDeleted(TimeSpan duration);
        void OnQueueMessageUpdated(TimeSpan duration);
        void OnQueueMessageAdded(TimeSpan duration);

        void OnCompressed(long decompressedLength, long compressedLength, TimeSpan duration);
        void OnDecompressed(long compressedLength, long decompressedLength, TimeSpan duration);

        void OnCompletedProcessDocumentJobDueToMissingDocument(ProcessDocumentMessage processDocumentMessage);

        void OnMissingSubscriptionFromSubscriptionStore(Guid userId, string uniqueId);
    }
}