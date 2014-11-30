using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Knapcode.KitchenSink.Support;
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

        public async Task<T> GetAsync(string partitionKey, string rowKey)
        {
            TableOperation operation = TableOperation.Retrieve<GenericTableEntity<T>>(partitionKey, rowKey);
            TableResult tableResult = null;
            TimeSpan duration = await EventTimer.TimeAsync(async () =>
            {
                tableResult = await _cloudTable.ExecuteAsync(operation);
            });
            _eventSource.OnFetchedRecordFromAzure(partitionKey, rowKey, duration);

            var record = tableResult.Result as GenericTableEntity<T>;
            if (record == null)
            {
                _eventSource.OnMissingRecordFromAzure(partitionKey, rowKey);
                return default(T);
            }

            return record.Content;
        }

        public async Task<IEnumerable<T>> ListAsync(string partitionKey, string rowKeyLowerBound, string rowKeyUpperBound)
        {

            string condition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            if (rowKeyLowerBound != null)
            {
                condition = TableQuery.CombineFilters(
                    condition,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, rowKeyLowerBound));
            }

            if (rowKeyUpperBound != null)
            {
                condition = TableQuery.CombineFilters(
                    condition,
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, rowKeyUpperBound));
            }
            
            TableQuery<GenericTableEntity<T>> query = new TableQuery<GenericTableEntity<T>>().Where(condition);

            TableQuerySegment<GenericTableEntity<T>> result = null;
            TimeSpan duration = await EventTimer.TimeAsync(async () =>
            {
                result = await _cloudTable.ExecuteQuerySegmentedAsync(query, null);
            });
            _eventSource.OnFetchedListFromAzure(partitionKey, result.Results.Count, duration);

            return result.Results.Select(r => r.Content);
        }

        public async Task SetAsync(string partitionKey, string rowKey, T value)
        {
            var entity = new GenericTableEntity<T>
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                Content = value
            };
            TableOperation operation = TableOperation.InsertOrReplace(entity);
            TimeSpan duration = await EventTimer.TimeAsync(async () =>
            {
                await _cloudTable.ExecuteAsync(operation);
            });
            _eventSource.OnSavedRecordToAzure(partitionKey, rowKey, duration);
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