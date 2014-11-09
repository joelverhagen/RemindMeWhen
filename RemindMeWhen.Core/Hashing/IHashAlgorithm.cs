namespace Knapcode.RemindMeWhen.Core.Hashing
{
    public interface IHashAlgorithm
    {
        string GetHash(byte[] buffer);
    }
}