namespace Knapcode.RemindMeWhen.Core.Support
{
    public interface IHashAlgorithm
    {
        Hash GetHash(byte[] buffer);
    }
}