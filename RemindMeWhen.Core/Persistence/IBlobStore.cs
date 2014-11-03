using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public interface IBlobStore
    {
        Task<byte[]> Get(string key);
        Task Set(string key, byte[] value);
    }
}