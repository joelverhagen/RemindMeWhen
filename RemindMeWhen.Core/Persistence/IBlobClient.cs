using System.Threading.Tasks;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public interface IBlobClient
    {
        IBlobContainer GetBlobContainer(string blobContainerName);
    }
}