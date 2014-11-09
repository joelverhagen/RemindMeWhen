using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public interface IKeyValueStore<in TKey, TValue>
    {
        Task<TValue> GetAsync(TKey key);
        Task SetAsync(TKey key, TValue value);
    }
}