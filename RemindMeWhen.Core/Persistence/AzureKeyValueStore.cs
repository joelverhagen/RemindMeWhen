using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public class AzureKeyValueStore<TValue> : IKeyValueStore<string, TValue>
    {
        private readonly CloudTable _cloudTable;
        private readonly IEventSource _eventSource;

        public AzureKeyValueStore(IEventSource eventSource, CloudTable cloudTable)
        {
            _eventSource = eventSource;
            _cloudTable = cloudTable;
        }

        public async Task<TValue> GetAsync(string key)
        {
            TableOperation operation = TableOperation.Retrieve<GenericTableEntity<TValue>>(key, key);
            TableResult tableResult;
            using (EventTimer.OnCompletion(d => _eventSource.OnFetchedKeyValueFromAzure(key, d)))
            {
                tableResult = await _cloudTable.ExecuteAsync(operation);
            }

            var record = tableResult.Result as GenericTableEntity<TValue>;
            if (record == null)
            {
                _eventSource.OnMissingKeyValueFromAzure(key);
                return default(TValue);
            }

            return record.Content;
        }

        public async Task SetAsync(string key, TValue value)
        {
            var entity = new GenericTableEntity<TValue>
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

        private class GenericTableEntity<T> : TableEntity
        {
            public T Content { get; set; }

            public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
            {
                Content = Activator.CreateInstance<T>();
                ReadUserObject(Content, properties, operationContext);
            }

            public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
            {
                return WriteUserObject(Content, operationContext);
            }
        }
    }
}