using System.Collections.Generic;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public class AzureTable<T> : ITable<T>
    {
        private readonly CloudTable _cloudTable;
        private readonly IEventSource _eventSource;

        public AzureTable(IEventSource eventSource, CloudTable cloudTable)
        {
            _eventSource = eventSource;
            _cloudTable = cloudTable;
        }

        public async Task<T> GetAsync(string key)
        {
            TableOperation operation = TableOperation.Retrieve<GenericTableEntity<T>>(key, key);
            TableResult tableResult;
            using (EventTimer.OnCompletion(d => _eventSource.OnFetchedKeyValueFromAzure(key, d)))
            {
                tableResult = await _cloudTable.ExecuteAsync(operation);
            }

            var record = tableResult.Result as GenericTableEntity<T>;
            if (record == null)
            {
                _eventSource.OnMissingKeyValueFromAzure(key);
                return default(T);
            }

            return record.Content;
        }

        public async Task SetAsync(string key, T value)
        {
            var entity = new GenericTableEntity<T>
            {
                PartitionKey = key,
                RowKey = key,
                Content = value
            };
            TableOperation operation = TableOperation.InsertOrReplace(entity);
            using (EventTimer.OnCompletion(d => _eventSource.OnSavedKeyValueToAzure(key, d)))
            {
                await _cloudTable.ExecuteAsync(operation);
            }
        }

        private class GenericTableEntity<TContent> : TableEntity
        {
            private const string ContentKey = "Content";

            public TContent Content { get; set; }

            public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
            {
                EntityProperty entityPropertyContent;
                if (!properties.TryGetValue(ContentKey, out entityPropertyContent) ||
                    entityPropertyContent.PropertyType != EdmType.String ||
                    entityPropertyContent.StringValue == null)
                {
                    return;
                }

                Content = JsonConvert.DeserializeObject<TContent>(entityPropertyContent.StringValue);
            }

            public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
            {
                string jsonContent = JsonConvert.SerializeObject(Content);
                var entityPropertyContent = new EntityProperty(jsonContent);
                return new Dictionary<string, EntityProperty> { {ContentKey, entityPropertyContent} };
            }
        }
    }
}