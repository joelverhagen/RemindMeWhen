using System.Threading.Tasks;
using Knapcode.RemindMeWhen.Core.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public class AzureTableClient : ITableClient
    {
        private readonly CloudTableClient _cloudTableClient;
        private readonly IEventSource _eventSource;

        public AzureTableClient(IEventSource eventSource, CloudTableClient cloudTableClient)
        {
            _eventSource = eventSource;
            _cloudTableClient = cloudTableClient;
        }

        public ITable<T> GetTable<T>(string tableName)
        {
            return new AzureTable<T>(_eventSource, _cloudTableClient.GetTableReference(tableName));
        }
    }
}