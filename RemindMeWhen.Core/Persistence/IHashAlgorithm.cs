namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public interface IHashAlgorithm
    {
        Hash GetHash(byte[] buffer);
    }
}