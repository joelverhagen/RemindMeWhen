using System.Security.Cryptography;
using Knapcode.RemindMeWhen.Core.Support;

namespace Knapcode.RemindMeWhen.Core.Hashing
{
    public class Sha256HashAlgorithm : IHashAlgorithm
    {
        public string GetHash(byte[] buffer)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] value = sha256.ComputeHash(buffer);
            return "SHA256-" + Base64Url.Encode(value);
        }
    }
}