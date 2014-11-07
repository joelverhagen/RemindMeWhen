using System.IO;
using System.IO.Compression;

namespace Knapcode.RemindMeWhen.Core.Support.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] Compress(this byte[] bytes)
        {
            var outputStream = new MemoryStream();
            using (var compressStream = new GZipStream(outputStream, CompressionMode.Compress))
            {
                compressStream.Write(bytes, 0, bytes.Length);
            }
            return outputStream.ToArray();
        }

        public static byte[] Decompress(this byte[] bytes)
        {
            var compressedStream = new MemoryStream(bytes);
            using (var decompressStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                var outputStream = new MemoryStream();
                decompressStream.CopyTo(outputStream);
                return outputStream.ToArray();
            }
        }
    }
}