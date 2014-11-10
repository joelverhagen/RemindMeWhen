using System;
using System.Globalization;
using Knapcode.RemindMeWhen.Core.Settings;
using Microsoft.Azure.WebJobs;

namespace Knapcode.RemindMeWhen.QueueJob
{
    public class NameResolver : INameResolver
    {
        public const string ProcessDocumentQueueNameKey = "ProcessDocumentQueueName";

        private readonly AzureStorageSettings _storageSetting;

        public NameResolver(AzureStorageSettings storageSetting)
        {
            _storageSetting = storageSetting;
        }

        public string Resolve(string name)
        {
            switch (name)
            {
                case ProcessDocumentQueueNameKey:
                    return _storageSetting.ProcessDocumentQueueName;
                default:
                    string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "The provided queue name key '{0}' is not recognized.",
                        name);
                    throw new ArgumentException(message, "name");
            }
        }
    }
}