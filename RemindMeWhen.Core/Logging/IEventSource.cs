using System;
using Knapcode.RemindMeWhen.Core.Clients;
using Knapcode.RemindMeWhen.Core.Models;
using Knapcode.RemindMeWhen.Core.Queue;

namespace Knapcode.RemindMeWhen.Core.Logging
{
    public interface IEventSource
    {
        void OnSearchedRottenTomatoesForMovies(string query, PageOffset pageOffset, TimeSpan duration);

        void OnMissingKeyValueFromAzure(string key);
        void OnFetchedKeyValueFromAzure(string key, TimeSpan duration);
        void OnSavedKeyValueToAzure(string key, TimeSpan duration);

        void OnMissingBlobFromAzure(string key);
        void OnBlobDownloadFromAzure(string key, long length, TimeSpan duration);
        void OnBlobUploadedToAzure(string key, long length, TimeSpan duration);
        void OnBlobExistenceCheckedInAzure(string key, bool exists, TimeSpan duration);

        void OnMissingDocumentMetadataFromDocumentStore(Guid documentMetadataId);
        void OnMissingDocumentFromDocumentStore(DocumentMetadata documentMetadata);
        void OnDuplicateFoundInDocumentStore(DocumentId documentId, string documentHash);
        
        void OnQueueMessagePeeked(TimeSpan duration);
        void OnQueueMessageFetched(TimeSpan duration);
        void OnQueueMessageDeleted(TimeSpan duration);
        void OnQueueMessageUpdated(TimeSpan duration);
        void OnQueueMessageAdded(TimeSpan duration);

        void OnCompressed(long decompressedLength, long compressedLength, TimeSpan duration);
        void OnDecompressed(long compressedLength, long decompressedLength, TimeSpan duration);

        void OnCompletedProcessDocumentDueToMissingDocument(QueueMessage<ProcessDocument> queueMessage);
    }
}