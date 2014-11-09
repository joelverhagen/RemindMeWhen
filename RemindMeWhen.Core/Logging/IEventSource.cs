using System;
using Knapcode.RemindMeWhen.Core.Identities;

namespace Knapcode.RemindMeWhen.Core.Logging
{
    public interface IEventSource
    {
        void OnSearchedRottenTomatoesForMovies(string query, int page, int pageLimit, TimeSpan duration);

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
    }
}