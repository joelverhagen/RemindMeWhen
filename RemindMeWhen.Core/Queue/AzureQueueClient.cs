using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Persistence;
using Knapcode.RemindMeWhen.Core.Settings;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Knapcode.RemindMeWhen.Core.Queue
{
    public class AzureQueueClient : IQueueClient
    {
        private readonly CloudQueueClient _cloudQueueClient;
        private readonly IDictionary<string, string> _queueNames;

        public AzureQueueClient(CloudQueueClient cloudQueueClient, AzureQueueSettings settings)
        {
            _cloudQueueClient = cloudQueueClient;
            _queueNames = new Dictionary<string, string>(settings.QueueNames.ToDictionary(p => p.Key, p => p.Value));
        }

        public async Task<IQueue<T>> GetQueueAsync<T>()
        {
            string fullTypeName = typeof (T).FullName;
            string queueName;
            if (!_queueNames.TryGetValue(fullTypeName, out queueName))
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "The provided queue message content type '{0}' does not have a configured queue name.",
                    fullTypeName);
                throw new ArgumentException(message);
            }

            CloudQueue cloudQueue = _cloudQueueClient.GetQueueReference(queueName);
            await AzureInitialization.CreateIfNotExistsAsync(cloudQueue);
            return new AzureQueue<T>(cloudQueue);
        }
    }
}