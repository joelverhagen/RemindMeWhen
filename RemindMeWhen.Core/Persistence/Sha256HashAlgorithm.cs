using System.Security.Cryptography;

namespace Knapcode.RemindMeWhen.Core.Persistence
{
    public class Sha256HashAlgorithm : IHashAlgorithm
    {
        public Hash GetHash(byte[] buffer)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] value = sha256.ComputeHash(buffer);

            return new Hash("SHA256", value);
        }
    }
}