using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public interface IBlobContainer
    {
        Task<byte[]> GetAsync(string key);
        Task SetAsync(string key, byte[] value);
        Task<bool> ExistsAsync(string key);
    }
}