namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public interface ITableClient
    {
        ITable<T> GetTable<T>(string tableName);
    }
}