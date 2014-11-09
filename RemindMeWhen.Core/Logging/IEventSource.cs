using System;
using Knapcode.RemindMeWhen.Core.Identities;
using Knapcode.RemindMeWhen.Core.Models;

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

        void OnMissingDocumentMetadataFromDocumentStore(DocumentIdentity identity, string metadataKey);
        void OnMissingDocumentFromDocumentStore(DocumentIdentity identity, string documentKey);
        void OnDuplicateFoundInDocumentStore(DocumentIdentity identity, string documentKey);

        void OnQueueMessagePeeked(TimeSpan duration);
        void OnQueueMessageFetched(TimeSpan duration);
        void OnQueueMessageDeleted(TimeSpan duration);
        void OnQueueMessageUpdated(TimeSpan duration);
        void OnQueueMessageAdded(TimeSpan duration);
    }
}