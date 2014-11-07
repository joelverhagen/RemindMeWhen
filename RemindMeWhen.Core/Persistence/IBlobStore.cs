using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public interface IBlobStore
    {
        Task<byte[]> GetAsync(string key);
        Task SetAsync(string key, byte[] value);
    }
}