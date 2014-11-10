using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Knapcode.RemindMeWhen.Core.Identities;
using Knapcode.RemindMeWhen.Core.Models;
using Newtonsoft.Json;

namespace Knapcode.RemindMeWhen.Core.Logging
{
    public class EventSource : IEventSource
    {
        public void OnSearchedRottenTomatoesForMovies(string query, PageOffset pageOffset, TimeSpan duration)
        {
            Log(new {query, pageOffset, duration});
        }

        public void OnMissingKeyValueFromAzure(string key)
        {
            Log(new {key});
        }

        public void OnFetchedKeyValueFromAzure(string key, TimeSpan duration)
        {
            Log(new {key, duration});
        }

        public void OnSavedKeyValueToAzure(string key, TimeSpan duration)
        {
            Log(new {key, duration});
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

        public void OnMissingDocumentMetadataFromDocumentStore(DocumentIdentity identity, string metadataKey)
        {
            Log(new {identity, metadataKey});
        }

        public void OnMissingDocumentFromDocumentStore(DocumentIdentity identity, string documentKey)
        {
            Log(new {identity, documentKey});
        }

        public void OnDuplicateFoundInDocumentStore(DocumentIdentity identity, string documentKey)
        {
            Log(new {identity, documentKey});
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

        public void OnCompletedProcessDocumentDueToMissingDocument(DocumentIdentity documentIdentity)
        {
            Log(new {documentIdentity});
        }

        public void OnCompressed(long decompressedLength, long compressedLength, TimeSpan duration)
        {
            Log(new {decompressedLength, compressedLength, duration});
        }

        public void OnDecompressed(long compressedLength, long decompressedLength, TimeSpan duration)
        {
            Log(new {compressedLength, decompressedLength, duration});
        }

        private static void Log(object obj, [CallerMemberName] string callerMemberName = null)
        {
            Trace.WriteLine(JsonConvert.SerializeObject(new {Content = obj, CallerMemberName = callerMemberName}));
        }
    }
}