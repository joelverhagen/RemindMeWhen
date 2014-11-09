using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public interface ITable<T>
    {
        Task<T> GetAsync(string key);
        Task SetAsync(string key, T value);
    }
}