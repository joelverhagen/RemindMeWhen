using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public static class AzureInitialization
    {
        private static readonly ConcurrentDictionary<string, bool> QueueNames = new ConcurrentDictionary<string, bool>();
        private static readonly ConcurrentDictionary<string, bool> TableNames = new ConcurrentDictionary<string, bool>();
        private static readonly ConcurrentDictionary<string, bool> BlobContainerNames = new ConcurrentDictionary<string, bool>();

        public static async Task CreateIfNotExistsAsync(CloudQueue c)
        {
            await CreateIfNotExistsAsync(c.Name, QueueNames, c.CreateIfNotExistsAsync);
        }

        public static async Task CreateIfNotExistsAsync(CloudTable c)
        {
            await CreateIfNotExistsAsync(c.Name, TableNames, c.CreateIfNotExistsAsync);
        }

        public static async Task CreateIfNotExistsAsync(CloudBlobContainer c)
        {
            await CreateIfNotExistsAsync(c.Name, BlobContainerNames, c.CreateIfNotExistsAsync);
        }

        private static async Task CreateIfNotExistsAsync(string name, ConcurrentDictionary<string, bool> dictionary, Func<Task> createIfNotExistsAsync)
        {
            if (!dictionary.ContainsKey(name))
            {
                await createIfNotExistsAsync();
                dictionary[name] = true;
            }
        }
    }
}